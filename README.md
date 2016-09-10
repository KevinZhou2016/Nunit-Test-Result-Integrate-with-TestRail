# Nunit-Test-Result-Integrate-with-TestRail
Nunit  Specflow Test Result Integrate with  TestRail

1.The automation project should be  base on Nunit+Specflow

2.Nunit version should be less than 2.6.4
3.Test Case should be looks like as below,The test caseId in feature file should be same as test rail

@caseid:1

Scenario: GetCityByCountry-Shanghai



When I  input Country name 'China' to get City
Then The cities should be contains 'Shanghai'