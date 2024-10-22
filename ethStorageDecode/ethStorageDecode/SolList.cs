﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static SolidityParser;

namespace ethStorageDecode
{
    public class SolList : SolidityBaseListener
    {
        //public List<StateVariableDeclarationContext> Lines = new List<StateVariableDeclarationContext>();
        //public List<VariableDeclarationContext> varList = new List<VariableDeclarationContext>();
        bool startVar = false;
        bool structStart = false;
        bool enumStart = false;       
        bool isarray = false;
        bool inFunction = false;
        int startMap = 0;
        bool isconst = false;  
        string typename;
        int typesize = 256;
        string name;

        SolidityStruct currStruct = null;
        SolidityEnum currEnum = null;
        SolidityMap currMap = null;

        Dictionary<string, SolidityStruct> structDefs = new Dictionary<string, SolidityStruct>();
        Dictionary<string, SolidityEnum> enumDefs = new Dictionary<string, SolidityEnum>();
        public List<SolidityVar> variableList = new List<SolidityVar>();
        public Dictionary<string,string> importList = new Dictionary<string, string>(); //class name is key, to raw import path
        public List<string> errorList = new List<string>();
        public List<string> searchpath;         
        string ethURL;
        string address;
        Dictionary<string, string> multiContracts;
    





        public SolList(List<string> _searchpath,  Dictionary<string,string> _multiContracts)
        {
            searchpath = _searchpath;
            multiContracts = _multiContracts;
            
        }
                public void MergeSubDecoder(SolList subdecoder)
        {
            variableList.AddRange(subdecoder.variableList);
        }

        public override void ExitInheritanceSpecifier([NotNull] InheritanceSpecifierContext context)
        {
            string importName = context.Start.Text+".sol";
            if (multiContracts != null && multiContracts.ContainsKey(context.Start.Text))
            {
                SolList subdecoder = solidtyDecoder.DecodeInoSubDecoder(multiContracts[context.Start.Text], address, ethURL, searchpath, multiContracts);
            }
            else
            {
                foreach (string filepath in searchpath)
                {
                    if (File.Exists(Path.Combine(filepath, importName)))
                    {
                        StreamReader txt = new StreamReader(Path.Combine(filepath, importName));
                        SolList subdecoder = solidtyDecoder.DecodeInoSubDecoder(txt.ReadToEnd(), address, ethURL, searchpath, multiContracts);
                        this.MergeSubDecoder(subdecoder);
                        return;
                    }
                }
            }
            errorList.Add("File for import not found :" + importName);
        }





        public override void ExitImportDirective([NotNull] ImportDirectiveContext context)
        {
            string importName = context.children[1].ToString().Replace("\"", "");
            foreach (string filepath in searchpath)
            {
                if (File.Exists(Path.Combine(filepath,importName)))
                {                    
                    return;
                }
            }             
            errorList.Add("File for import not found :" + importName);
            
            
        }



        

        public override void EnterInheritanceSpecifier([NotNull] InheritanceSpecifierContext context)
        {
           
        }


        public override void EnterFunctionCall([NotNull] FunctionCallContext context)
        {
            inFunction = true;
            ethGlobal.DebugPrint("Entering function call " + context.Start.Text);
        }

        public override void ExitFunctionCall([NotNull] FunctionCallContext context)
        {
            inFunction = false;
            ethGlobal.DebugPrint("->Exit function call " + context.Start.Text);
        }

        public override void EnterEnumDefinition([NotNull] EnumDefinitionContext context)
        {
            //string nm = context.Start.Text;
            enumStart = true;
            currEnum = new SolidityEnum();
        }

        public override void ExitEnumDefinition([NotNull] EnumDefinitionContext context)
        {
            enumDefs.Add(currEnum.name, currEnum);
            enumStart = false;
            currEnum = null;
            ethGlobal.DebugPrint("->Exit enum value " + context.Start.Text);

        }

        public override void EnterEnumValue([NotNull] EnumValueContext context)
        {
            enumStart = false;
            currEnum.AddEnum(context.Start.Text);
            ethGlobal.DebugPrint("Enter enum value " + context.Start.Text);


        }

        public override void EnterStateVariableDeclaration([NotNull] StateVariableDeclarationContext context)
        {
            // base.EnterStateVariableDeclaration(context);
            //object typenm = context.typeName();
            if (context.Payload.ToString().Contains(SolidityParser.ConstantKeyword.ToString()))
                isconst = true;
            startVar = true;
            structStart = false;
            ethGlobal.DebugPrint("Entering declarartion of " +  context.Start.Text);
            

            //Lines.Add(context);

        }

        public override void EnterStateMutability([NotNull] StateMutabilityContext context)
        {
            string payload = context.Payload.ToString();            
            if (context.Payload.ToString().Contains(SolidityParser.ConstantKeyword.ToString()))
                isconst = true;

        }

        public override void ExitStateMutability([NotNull] StateMutabilityContext context)
        {

            if (context.Start.Text == "constant")
            {
                isconst = true;
            }
        }






        public override void ExitStateVariableDeclaration([NotNull] StateVariableDeclarationContext context)
        {
            base.ExitStateVariableDeclaration(context);
            startVar = false;                    
            if (!isconst)
                parseVariables(context);
            isconst = false;
            //Console.WriteLine("-> Exit declarartion of " + context.Start.Text);
        }

        List<string> varlist = new List<string>();

        public override void EnterIdentifier([NotNull] IdentifierContext context)
        {
            if (startVar || structStart || enumStart)
            { 
                name = context.Start.Text;
                if (structStart)
                    currStruct.Name = name;
                if (enumStart)
                    currEnum.name = name;
                varlist.Add(name);

            }
        }
        


        public override void EnterExpressionList([NotNull] ExpressionListContext context)
        {
            
            if (startVar)
            {
                string brk = context.Start.Text;
            }
        }

        public override void EnterPrimaryExpression([NotNull] PrimaryExpressionContext context)
        {
           
            if (context.Parent != null && context.Parent.Parent != null)
            {
                
                int end = context.Start.StopIndex;
                int start = context.Start.StartIndex - context.Start.Column;
                string text = context.Start.InputStream.GetText(new Interval(start, end));
                if (text.Contains("constant"))
                    isconst = true;
            }

        }





        Regex numMatch = new Regex(@"([a-z]+)(\d+)");

        public override void EnterTypeName([NotNull] TypeNameContext context)
        {
            

            if (startVar)
            {
                 typename = context.Start.Text;  //if type name hit twice its an array
                if (numMatch.IsMatch(typename))
                {
                    Match numberMatch = numMatch.Match(typename);
                    typename = numberMatch.Groups[1].ToString();
                    try
                    {
                        typesize = Int32.Parse(numberMatch.Groups[2].ToString());
                    }
                    catch
                    {
                        errorList.Add("Error parsing number for " + typename);
                    }
                }
                else
                    typesize = 256;
                 int chkindex = context.Start.StopIndex + 1;
                 string txtat = context.Start.InputStream.GetText(new Interval(chkindex, chkindex));
                  
                  
                if (txtat.Equals("["))
                    isarray = true;
                
            }
        }

        



        public override void EnterIdentifierList([NotNull] IdentifierListContext context)
        {
            base.EnterIdentifierList(context);
            //string txtat = context.Start.InputStream.GetText(new Interval(chkindex, chkindex));
            if(context.Parent!=null && context.Parent.Parent!=null)
            {
                int start = ((IToken)context.Parent.Parent).StopIndex;
                int end = context.Start.StopIndex;
                string text = context.Start.InputStream.GetText(new Interval(start, end));
            }
            string st = context.Start.Text;
        }



        public override void EnterVariableDeclaration([NotNull] VariableDeclarationContext context)
        {
            base.EnterVariableDeclaration(context);
            startVar = true;
        }

        public override void ExitVariableDeclaration([NotNull] VariableDeclarationContext context)
        {
            base.ExitVariableDeclaration(context);
            startVar = false;
            isarray = false;
            if(currStruct!=null)
                parseVariables(context);           
        }

        public override void EnterStructDefinition([NotNull] StructDefinitionContext context)
        {
            base.EnterStructDefinition(context);
            structStart = true;
            currStruct = new SolidityStruct();
            //Console.WriteLine("Entering struct def "+ context.Start.Text);
        }

        public override void EnterModifierList([NotNull] ModifierListContext context)
        {
           
        }

    

        public override void EnterMapping([NotNull] MappingContext context)
        {
            base.EnterMapping(context);
            startMap ++;
            
        }

        public override void ExitMapping([NotNull] MappingContext context)
        {
            base.ExitMapping(context);
            //startMap = false;
        }

        



        public override void ExitStructDefinition([NotNull] StructDefinitionContext context)
        {
            base.ExitStructDefinition(context);
            structDefs.Add(currStruct.Name, currStruct);
            currStruct = null;
            structStart = false;
            //Console.WriteLine("->Exit struct def");
        }

        

        public override void ExitAssemblyAssignment([NotNull] AssemblyAssignmentContext context)
        {
            base.ExitAssemblyAssignment(context);
            structStart = false;
        }

        private void parseVariables(ParserRuleContext context)
        {
            if (inFunction) return;
            SolidityVar currentVar = null;
            switch (typename)
            {
                case "address":
                    currentVar = new SolidityAddress(name);
                    break;
                case "uint":
                    currentVar = new SolidityUint(typesize,name);
                    break;                
                case "string":
                    currentVar = new SolidityStringVar(name);
                    break;
                case "bool":
                    currentVar = new SolidityUintBool(name);
                    break;
                default:
                    if (structDefs.ContainsKey(typename))
                    {

                        currentVar = (SolidityVar)structDefs[typename].Clone();

                    }
                    else if (enumDefs.ContainsKey(typename))
                    {
                        currentVar = (SolidityVar)enumDefs[typename].Clone();
                    }
                    else
                    {
                        //  throw new NotSupportedException("unknwon type " + typename);
                        Console.WriteLine(" uknown type " + typename + " at adding as address" +context.Start.Line);
                        currentVar = new SolidityAddress(name);
                    }
                    break;
            }
            if(isarray)
            {
                SolidityVar oldvar = currentVar;
                currentVar = new SolidityArray(currentVar, name);
                isarray = false;
            }
            if (currStruct != null && currentVar!=null)
                currStruct.AddType(currentVar);
            else if (currentVar != null)
            {
                if(startMap>0)
                {
                    SolidityMap mapvar = new SolidityMap(currentVar, name,startMap);
                    /*for(int i = 1; i < startMap; i++)
                    {
                        SolidityMap newMap = new SolidityMap(mapvar, "");
                        mapvar = newMap;
                    }
                    mapvar.name = name;*/
                    variableList.Add(mapvar);
                    startMap = 0;
                }
                else
                    variableList.Add(currentVar);
                ethGlobal.DebugPrint("added " + name);
            }
        }
            

        

       


        /* NameContext name = context.name();
         OpinionContext opinion = context.opinion();
         SpeakLine line = new SpeakLine() { Person = name.GetText(), Text = opinion.GetText().Trim('"') };
         Lines.Add(line);      
        return line;*/
    }




 /*   public abstract class SolidityVar
    {

        public abstract long Lenght();

        public abstract DecodedOutput decode(object obj);

        public abstract DecodedOutput decoded(object obj, object key);

        public abstract bool IsKeyNeeded();
        



        
    }*/

  /*  public class SolidityStruct : SolidityVar
    {
        List<SolidityVar> internalVars;
        long len = 0;

        public SolidityStruct(List<SolidityVar> vars)
        {
            internalVars = vars;
            foreach(var v in internalVars )
            {
                len += v.Lenght();
            }
        }

        public override 


        public override DecodedOutput decode(object obj)
        {
           
        }

        public override DecodedOutput decoded(object obj, object key)
        {
            throw new NotImplementedException();
        }

        public override bool IsKeyNeeded()
        {
            throw new NotImplementedException();
        }

        public override long Lenght()
        {
            return len
        }
    }

    public class SolidityUint : SolidityVar
    {
        string Name { get; }
        long len = 0;

        public SolidityUint(string name)
        {
            Name = name;
        }

        public override


        public override DecodedOutput decode(object obj)
        {

        }

        public override DecodedOutput decoded(object obj, object key)
        {
            throw new NotImplementedException();
        }

        public override bool IsKeyNeeded()
        {
            throw new NotImplementedException();
        }

        public override long Lenght()
        {
            return len
        }
    }



    public class global
    {
        //List<SolidityVar> structDefs;
        //struct regex ($\s*struct(.+?)\{([^\}]+?)}) 
        static Regex structmatch = new Regex(@"($\s*struct(.+?)\{([^\}]+?)}");
        static Regex globalMatch = new Regex(@"((struct.+?\{(.+?)\})|(function.+?\{.+?\})|(?<v1>^\s*(?<v2>\w+)(?<v3>.+?)\;))", RegexOptions.Multiline | RegexOptions.Singleline);
        public static Dictionary<string, SolidityVar> structDefs = new Dictionary<string,SolidityVar>();
        public static List<SolidityVar> VarFactory(string input, Dictionary<string,SolidityVar> currentStructs)
        {
            List<SolidityVar> currentVariables = new List<SolidityVar>();
            MatchCollection coll = globalMatch.Matches(input);
            foreach(Match m in coll) 
            {
                if (m.Groups[1].ToString().Contains("function"))
                    continue;
                if(m.Groups[1].ToString().Contains("struct"))
                {
                    Match m2 = structmatch.Match(m.Groups[1].ToString());
                    currentVariables.Add(DecodeStruct(m2.Groups[3].ToString(), currentStructs));
                }
                if(m.Groups[1].ToString().Contains("uint"))
                {

                }
                else if(m.Groups[1].ToString().Contains("byes32"))
                { 

                }
                
            }
        }

        public static SolidityStruct DecodeStruct(string contents,Dictionary<string,SolidityVar> currentStruct)
        {
            return new SolidityStruct(VarFactory(contents, currentStruct));
        }

    }

    //uint, long, bytes32, struct ,map, array

    public class DecodedOutput
    {
        string val
        public DecodedOutput(string _val)
        {
            val = _val;
        }
        public bool IsSimple()
        {
            return true;
        }

        public string GetDecode()
        {
            return val;
        }

        public string GetDecode(string key)
        {
            throw new NotSupportedException("Simple Decoded output does not have keys");
        }
    }

    public class ComplexeDecodedOutput
    {
        int ind;
        public ComplexeDecodedOutput(int startInd)
        {
            ind = startInd;
        }
        public bool IsSimple()
        {
            return true;
        }

        public string GetDecode()
        {
            throw new NotSupportedException("Must have keys for complex decoded type");
        }
    }

        public string GetDecode(string key)
        {
             return "";
        }
    }

    public class SolidityVariableDecoder
    {

        //Regex expr = new Regex(@".?+(\w+)\s(\w+\s)?(\w+).*;");
       

        public SolidityVariableDecoder(string file)
        {
           MatchCollection matches=  globalMatch.Matches(file);
           List<SolidityVar> varList;
           foreach(Match m in matches)
           {
                varList.Add(SolidityVar.Create(m));
           }
        }



    } */


}

