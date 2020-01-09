pragma solidity >=0.4.21 <0.6.0;


contract testClassSimple
{
    uint testUint;
    string testString;
    uint[] uintArray;
    string[] stringArray;
    struct SimpleStruct
    {
        uint simpleStructVal;
        string simpleStructString;      
    }    
    string[] stringArray2; 
	SimpleStruct[] arrayStruct;
    mapping(uint=>uint) simpleMap;
    mapping(uint=>SimpleStruct) structMap;
    mapping(uint=>mapping(uint=>SimpleStruct)) complexMap;

    constructor() public
    {
        testUint=8;
        testString="test string ";
        uintArray.push(12);
        uintArray.push(14);
        uintArray.push(15);
        stringArray.push("test1");
        stringArray.push("rain");
        stringArray.push("in");
        stringArray.push("stays mainly");
        stringArray.push("in the plains");
        //struct1.simpleStructVal = 42;
        //struct1.simpleStructString = "the answer to everything";
        simpleMap[4] = 42;
        simpleMap[8] = 42;
        simpleMap[10] = 42;
		arrayStruct.push( SimpleStruct(10,"Test struct 1 in array"));
		arrayStruct.push( SimpleStruct(20,"Test struct 2 in array"));
		arrayStruct.push( SimpleStruct(30,"Test struct 3 in array"));   
        structMap[4] = SimpleStruct(14,"mapping struct 1");
        structMap[1] = SimpleStruct(21,"mapping struct 2");
        complexMap[1][2] = SimpleStruct(12,"complex struct 1 2");
        complexMap[2][2] = SimpleStruct(22,"complex struct 2 2");

     }
}