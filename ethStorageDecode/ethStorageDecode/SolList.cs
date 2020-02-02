using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
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
        
       


        public SolList(List<string> _searchpath, string _address, string _ethURL)
        {
            searchpath = _searchpath;
        }

        public void MergeSubDecoder(SolList subdecoder)
        {
            variableList.AddRange(subdecoder.variableList);
        }





        public override void ExitImportDirective([NotNull] ImportDirectiveContext context)
        {
            string importName = context.children[1].ToString().Replace("\"", "");
            foreach (string filepath in searchpath)
            {
                if (File.Exists(Path.Combine(filepath,importName)))
                {
                    SolList subdecoder = solidtyDecoder.DecodeInoSubDecoder(Path.Combine(filepath, importName), address, ethURL, searchpath);
                    this.MergeSubDecoder(subdecoder);
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
            Console.WriteLine("Entering function call " + context.Start.Text);
        }

        public override void ExitFunctionCall([NotNull] FunctionCallContext context)
        {
            inFunction = false; 
            Console.WriteLine("->Exit function call " + context.Start.Text);
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
            Console.WriteLine("->Exit enum value " + context.Start.Text);

        }

        public override void EnterEnumValue([NotNull] EnumValueContext context)
        {
            enumStart = false;
            currEnum.AddEnum(context.Start.Text);
            Console.WriteLine("Enter enum value " + context.Start.Text);


        }

        public override void EnterStateVariableDeclaration([NotNull] StateVariableDeclarationContext context)
        {
            // base.EnterStateVariableDeclaration(context);
            //object typenm = context.typeName();
            if (context.Payload.ToString().Contains(SolidityParser.ConstantKeyword.ToString()))
                isconst = true;
            startVar = true;
            structStart = false;
            Console.WriteLine("Entering declarartion of " +  context.Start.Text);
            

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
        

        public override void ExitIdentifier([NotNull] IdentifierContext context)
        {
            if(startVar)
            {
                string tst = context.Stop.Text;
            }
        }

        public override void EnterExpression([NotNull] ExpressionContext context)
        {
            //base.EnterExpression(context);
            if(startVar)
            {
                string brackets = context.Start.Text;
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


        




        public override void EnterTypeName([NotNull] TypeNameContext context)
        {
            

            if (startVar)
            {
                 typename = context.Start.Text;  //if type name hit twice its an array
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
                    currentVar = new SolidityUint(32,name);
                    break;
                case "uint8":
                    currentVar = new SolidityUint(8,name);
                    break;
                case "uint16":
                    currentVar = new SolidityUint(16,name);
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
                        Console.WriteLine(" uknown type " + typename + " at " +context.Start.Line);
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
                Console.WriteLine("added " + name);
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

