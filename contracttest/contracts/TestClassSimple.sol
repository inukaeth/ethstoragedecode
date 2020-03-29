pragma solidity >=0.4.21 <0.6.2;


contract testClassSimple
{
    uint testUint; //0
    string testString; //1
    uint[] uintArray; //2
    string[] stringArray; //3
    struct SimpleStruct //
    {
        uint simpleStructVal;
        string simpleStructString;
    }
    string[] stringArray2; //4
	SimpleStruct[] arrayStruct; //5
    mapping(uint=>uint) simpleMap; //6
    mapping(uint=>SimpleStruct) structMap; //7
    mapping(uint=>mapping(uint=>SimpleStruct)) complexMap; //8

    constructor() public
    {
        testUint = 8;
        testString = "test string ";
        uintArray.push(12);
        uintArray.push(14);
        uintArray.push(15);
        stringArray.push("test1");
        stringArray.push("rain");
        stringArray.push("in");
        stringArray.push("stays mainly");
        stringArray.push("in the plains");
        simpleMap[4] = 42;
        simpleMap[8] = 42;
        simpleMap[10] = 42;
		arrayStruct.push(SimpleStruct(10,"Test struct 1 in array"));
		arrayStruct.push(SimpleStruct(20,"Test struct 2 in array"));
		arrayStruct.push(SimpleStruct(30,"Test struct 3 in array"));
        structMap[4] = SimpleStruct(14,"mapping struct 1");
        structMap[1] = SimpleStruct(21,"mapping struct 2");
        complexMap[1][2] = SimpleStruct(12,"complex struct 1 2");
        complexMap[2][2] = SimpleStruct(22,"complex struct 2 2");

     }
}