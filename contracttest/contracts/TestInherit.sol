pragma solidity >=0.4.21 <0.6.0;
import "./TestClassSimple.sol";


contract testInherit is testClassSimple
{
    uint mainclass;

    constructor() public
    {
        mainclass=52;
    }
}