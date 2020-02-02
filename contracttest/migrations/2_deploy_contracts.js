var testClass = artifacts.require("./TestClassSimple.sol");
var inherit = artifacts.require("./TestInherit.sol")

module.exports = function(deployer) {
  deployer.then(async () => {
      var test1 = await deployer.deploy(testClass);
      var t2 = await deployer.deploy(inherit);
      
  });
}
