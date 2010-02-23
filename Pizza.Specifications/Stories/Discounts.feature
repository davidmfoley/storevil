Discounts

Scenario: two mediums for 20 bucks special
When customer orders 2 medium cheese pizzas
Grand Total should be $20

Scenario: two slices and a soda promo... cheese
When customer orders 2 cheese slices and a coke
Grand total should be $6

Scenario: two slices and a soda promo... combo
When customer orders 2 combo slices and a coke
Grand total should be $6

Scenario: 10% discount if ordering 5 or more pizzas
When customer orders 5 large cheese pizzas 
Grand total should be $67.50

Scenario: beer should be $4
When customer orders a beer 
Grand total should be $4

Scenario: pizza and beer special should be $6 (failing test)
When customer orders a beer 
and orders a slice
# uncomment this to see an example of a failure
# Grand total should be $6

