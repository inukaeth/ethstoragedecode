using ethStorageDecode;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ethStorageDecodeTest
{
    public class TestUtility
    {

        public static void CheckVariable(string name, string value, int index, List<DecodedContainer> variableList)
        {
            Assert.IsTrue(variableList[index].solidityVar.name == name &&
                   variableList[index].decodedValue == value,
                   string.Format("Error variable or value not matched on CheckVariable got {0} {1} instead of {2} {3}", variableList[index].solidityVar.name,
                    variableList[index].decodedValue, name, value));


        }

        public delegate bool CheckFunction(DecodedContainer decodedContainer, CheckValues checkValues);

        public class NameValues
        {
            public string Name;
            public string Value;
        }


        public class CheckValues
        {
            public string parentName;
            public string value;
            public List<NameValues> nameValues;
            public int checkIndex;
        }

        public static bool StructCheckFunction(DecodedContainer parentContainer, CheckValues structValues)
        {

            if (parentContainer.children.Count > structValues.checkIndex)
            {
                for (int i = 0; i < structValues.nameValues.Count; i++)
                {
                    if (!(parentContainer.children[i].solidityVar.name == structValues.nameValues[i].Name &&
                         parentContainer.children[i].decodedValue == structValues.nameValues[i].Value))
                        return false;

                }
                return true;
            }
            return false;

        }

        public static bool CheckValueFunction(DecodedContainer parentContainer, CheckValues checkValues)
        {
            return parentContainer.solidityVar.name == checkValues.parentName &&
                   parentContainer.decodedValue == checkValues.value;

        }

        public static void CheckArrayItem(int index, CheckFunction checkFn, DecodedContainer arrayContainer,
            CheckValues checkValues)
        {
            if (arrayContainer.children.Count > index)
            {
                Assert.IsTrue(checkFn(arrayContainer.children[index], checkValues),
                    string.Format("Error check array failed on check function for array {0} length {1}",
                    arrayContainer.solidityVar.name, arrayContainer.children.Count));

            }
            else
                Assert.Fail("Error array check failed index is {0} but count is {1}", index, arrayContainer.children.Count);
        }


        public static void CheckMapItem(string Index, CheckFunction checkFn, DecodedContainer arrayContainer,
            CheckValues checkValues)
        {
            Assert.IsTrue(arrayContainer.solidityVar.name == checkValues.parentName, "Error the parent name does not match it should be "+ arrayContainer.solidityVar.name);
            DecodedContainer current = arrayContainer;
            current = current.children.Find(a => a.key == Index);
            Assert.IsTrue(current != null, "Error map check failed index is {0} but count is {1}", Index, current.children.Count);
            Assert.IsTrue(checkFn(current, checkValues), "Check function failed for " + arrayContainer.solidityVar.name);
        }

       


        
    }
}
