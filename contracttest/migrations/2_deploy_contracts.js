var testClass = artifacts.require("./TestClassSimple.sol");

module.exports = function(deployer) {
  deployer.then(async () => {
      var test1 = await deployer.deploy(testClass);
      
  });
}
