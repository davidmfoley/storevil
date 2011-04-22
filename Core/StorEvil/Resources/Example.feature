As someone new to StorEvil
I want to see some examples
So that I can learn how it works

Scenario: Example passing scenario					
	Given some condition
	When I take an action
	Then the state should be valid
	
Scenario: Example failing scenario
	Given some condition
	When I take another action
	Then the state should be invalid
	
Scenario: Example pending scenario
	Given some condition
	When I have some action that has not yet been implemented
	# it will stop here because it can not interpret
	
Scenario Outline: Example of a scenario outline
	the numbers <first> and <second>
	should add to <expected>
	Examples:
	|first|second|expected|
	| 1   | 2    | 3      |
	| 4   | 6    | 10     |
	| 2   | 2    | 4      |

Scenario: Example of arrays
	These numbers should add as expected:
	|First|Second|Expected|
	| 1   | 2    | 3      |
	| 4   | 6    | 10     |
	| 2   | 2    | 4      |