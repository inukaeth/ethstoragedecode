pragma solidity >=0.5 <0.6.0;


import "../installed_contracts/zeppelin/contracts/token/ERC20/ERC20.sol";
import "../installed_contracts/Strings.sol";
import "./BaseLoanContract.sol";
import "./ShortTermLoanPool.sol";
import "./USDStableCoin.sol";
import "./PoolPay.sol";

contract LongTermLoanPool is ERC20, IPoolPay
{
    address[] LoanList;
    address[] public investors;
    mapping(address=>bool) investorExits;
    uint public loancount=0 ;
    string public loanclass ;
    uint public timestamp;
    uint public maturretime;
    address public owner;
    address public manager;
    address public shortTermPool;

    /*ERC20 stuff make this variable off the contructor */
    string public name ;
    string public symbol ; 
    uint public createTime ;   
    uint8 public constant decimals = 18;
    uint public amount;    
    uint public interest;
    uint public durationDays;
    uint public classIndex;
    uint public status=0; //0 open , 1 closed, 2 burned
    uint public totalMonthlyPaymentsFromLoanContracts=0;  
    bool public burned=false; 
    address trueUSD;
    event PayLongTerm(uint amount);
    event DisburseLongTerm(uint amount);
    event LongTermPoolBurned(address pooladdr);


    constructor(string memory _loanclass,string memory _name,uint _classIndex, address _manager, 
        address _shortTermPool,address usdcoin, uint _amount,
         uint _interest, uint _durationDays, uint accuracys) public
    {
        loanclass = _loanclass;
        manager = _manager;
        shortTermPool = _shortTermPool;
        createTime = now;
        //string memory space = " ";
        //name = _name;
         name = _name;
        //name = name.concat(_loanclass);
        symbol = "LTP";     
        amount = _amount;
        interest = _interest;
        durationDays= _durationDays;       
        trueUSD = usdcoin;      
        classIndex = _classIndex;
        _mint(address(this),amount);
    }

    


    function GetClassIndex() public returns(uint)
    {
        return classIndex;
    }

    function purchaseAmountOrBalance(uint amt) public
    {        
        uint amountusd = amt;
        uint amountPurchase = balanceOf(address(this));
        if(amountPurchase==0) return;
        if(amountPurchase<amountusd)
            amountusd = amountPurchase;
        require(!investorExits[msg.sender], "Investor has purchased coins previously");
        //require(this._balances[address(this)]>=amountusd,"long term pool does not have enough coins available for purchase");
        _transfer(address(this),msg.sender, amountusd);
        USDStableCoin coin = USDStableCoin(trueUSD);
        require(coin.allowance(msg.sender,address(this))>=amountusd,"Error there is no allowance for ltp");     
        coin.transferFrom(msg.sender,address(this),amountusd);   
        investorExits[msg.sender] = true;
        investors.push(msg.sender);

    }

    //To pay to the pool, currently pass through
    //we need to make this more efficient by paying on a monthyly or weekly basis
    function Pay(uint payamount) public
    {       
        if(status==0 || status==2)
            return;
        totalMonthlyPaymentsFromLoanContracts+=payamount;
        USDStableCoin coin = USDStableCoin(trueUSD);
        require(coin.allowance(msg.sender,address(this))>=payamount,"Error there is not enough allowance");
        coin.transferFrom(msg.sender,address(this),payamount);  
        emit PayLongTerm(payamount);
    }


    //called from oracle to pay off 
    function DisburseMonthlyPaymentsToInvsetors() public 
    {
        if(status==0 || status==2)
            return;
        USDStableCoin coin = USDStableCoin(trueUSD);
        if(burned) return;

        //amount is the total number of tokens  
        uint total=0;
        uint bal = coin.balanceOf(address(this));
        for(uint i =0;i< investors.length; i++)
        {
            uint currbalance = balanceOf(investors[i]);
            total+=((bal*currbalance)/amount);
            coin.transfer(investors[i], ((bal*currbalance)/amount) );
        }
        if(total==0)
        {
            /*bool hasbalance=false;
            for(uint i2 =0;i2< LoanList.length; i2++)
            {
                BaseLoanContract bs = BaseLoanContract(LoanList[i2]);
                if(bs.GetBalance()!=0)
                {
                    hasbalance=true;
                    break;
                }

            }
            if(!hasbalance) 
            {*/
                for(uint i =0;i< investors.length; i++)
               {
                    uint currbalance = balanceOf(investors[i]);
                   _burn(investors[i], currbalance);
                }
                burned = true;
                status=2;
                emit LongTermPoolBurned(address(this));
            //}
           
        }
        totalMonthlyPaymentsFromLoanContracts = 0;
        emit DisburseLongTerm(bal);
    }

    
    function DemoBurn() public
    {
          for(uint i =0;i< investors.length; i++)
               {
                    uint currbalance = balanceOf(investors[i]);
                   _burn(investors[i], currbalance);
                }
                burned = true;
                status=2;
                emit LongTermPoolBurned(address(this));
    }
   

    function Poke() public
    {
        //is interest needed , pay off interest in that case
        //is the loan mature, if sos pay off and close
        //do any of the loans require liquidation, if so liquidate
        if(balanceOf(address(this))==0 && status==0)
        {
            payAndGetContracts();            
        }
    }

    function payAndGetContracts() private
    {
        ShortTermLoanPool sht= ShortTermLoanPool(shortTermPool);
        USDStableCoin coin = USDStableCoin(trueUSD);
        coin.approve(shortTermPool,amount);
        sht.GetPayAndTransferLoanContracts(amount);
        
    }

    function AddLoanContract(address contractaddr) public
    {
        require(msg.sender ==shortTermPool, "Can only be called by short term pool");
        //uint len  = LoanList.length++;
        //LoanList[len] = contractaddr;
        status = 1;
        LoanList.push(contractaddr);
    }


    function Activate() public
    {
        require(msg.sender== shortTermPool);


    }



    function GetAllLoanContracts() public view returns(address[] memory loans) 
    {
        loans = new address[](LoanList.length);
        for(uint i=0;i<LoanList.length;i++)
          loans[i] = LoanList[i];
    }

    function GetAllInvestorss() public view returns(address[] memory investlist)
    {
        investlist = new address[](investors.length);
        for(uint i=0; i< investors.length;i++)
        {
            investlist[i] = investors[i];
        }
    }


   /* function AuctionLoan() public
    {

    }

    function AuctionAsset() public
    {
        
    }

    function DisberseInterest() public
    {
        
    }

    function DisbersePrinciple() public
    {

    } */

    




}