pragma solidity >=0.4.21 <0.6.0;


contract testPacking
{
    uint8 slot0off0;  //8  slot 0
    uint32 slot0off8;  //40
    uint128 slot0off40;  //168
    uint128 slot1off0;  //not enough room move to slot 1
    uint32 slot1off129;   // slot 1 offset of 128, 
    struct SimpleStruct
    {
        uint8 slot0off0;
        uint128 slot0off8;
        uint128 slot1off130;
    }    
    uint8[] array8; 
	SimpleStruct[] arrayStruct;
    mapping(uint8=>uint8) simpleMap;
    mapping(uint8=>SimpleStruct) structMap;
    //mapping(uint=>mapping(uint=>SimpleStruct)) complexMap;

    constructor() public
    {
        slot0off0=19;
        slot0off8=21;
        slot0off40=54;
        slot1off0=57;
        slot1off129=96;
        array8.push(12);
        array8.push(14);
        array8.push(15);
     
        //struct1.simpleStructVal = 42;
        //struct1.simpleStructString = "the answer to everything";
        simpleMap[4] = 43;
        simpleMap[8] = 59;
        simpleMap[10] = 42;
        simpleMap[12] = 45;
		arrayStruct.push( SimpleStruct(23,14,25));
		arrayStruct.push( SimpleStruct(20,18,21));
		arrayStruct.push( SimpleStruct(30,33,34));   
        structMap[4] = SimpleStruct(14,19,29);
        structMap[1] = SimpleStruct(21,2,231);       

     }
}