Basic Grammar

Any text at the beginning of the file is not executed, but serves as a description of the feature or story 
that is tested in the features below.

Scenarios start with the word "Scenario" followed by a colon.

Scenario: basic matching of plaintext line
	When my name is StorEvil

# to insert explanatory text in the scenarios, use a comment
# comments are prefixed with the # character

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

	   
Scenario Outline: An outline is a scenario that is run once for each example
	Given I am <age> years old
	My age in one year should be <expected>

	Examples:
	|age|expected|
	|42 | 43	 |
	|35 | 36	 |
	|1  | 2      |
