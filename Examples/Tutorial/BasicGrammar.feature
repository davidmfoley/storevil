Learning StorEvil 

Basic Grammar

Scenario: basic matching of plaintext line
	When my name is StorEvil

Scenario: a basic assertion
	When my name is StorEvil
	I should eat all other BDD frameworks

Scenario: assertion using the ShouldXXX methods
	When my name is StorEvil
	All other BDD frameworks are eaten should be true
	
Scenario: parameterizing and adding a basic assertion
	When my name is "Some other BDD framework"
	All other BDD frameworks are eaten should be false
    
Scenario: matching with an integer parameter in the middle
	Given I am 42 years old
	My age in one year should be 43

