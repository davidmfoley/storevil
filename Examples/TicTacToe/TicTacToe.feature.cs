namespace TicTacToe{
[NUnit.Framework.TestFixtureAttribute] public class StoryTicTacToeRules : StorEvil.CodeGeneration.TestFixture {
   public object Contexts { get { return base.GetContexts();}}    [NUnit.Framework.SetUpAttribute]
    public void HandleSetUp() { base.BeforeEach(); }
    [NUnit.Framework.TestFixtureSetUpAttribute]
    public void HandleTestFixtureSetUp() { SetListener(new StorEvil.CodeGeneration.NUnitListener()); base.BeforeAll(); }
    [NUnit.Framework.TearDownAttribute]
    public void HandleTearDown() { base.AfterEach(); }
    [NUnit.Framework.TestFixtureTearDownAttribute]
    public void HandleTestFixtureTearDown() { base.AfterAll(); }
  [NUnit.Framework.TestAttribute] public void Xisfirstplayer() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 4
ExecuteLine(@"Given a new game");
#line hidden

#line 5
ExecuteLine(@"Then the current player should be X");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Gamestatemovebymove() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 8
ExecuteLine(@"Given a new game");
#line hidden

#line 9
ExecuteLine(@"When X plays in the top left");
#line hidden

#line 10
ExecuteLine(@"Then the current player should be O");
#line hidden

#line 11
ExecuteLine(@"And the board state should be
|X| | |
| | | |
| | | |");
#line hidden

#line 15
ExecuteLine(@"When O plays in the bottom right");
#line hidden

#line 16
ExecuteLine(@"Then the board state should be
|X| | |
| | | |
| | |O|");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Owins() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 22
ExecuteLine(@"Given the following board:
|X|X|O|
|X|O| |
| | | |");
#line hidden

#line 26
ExecuteLine(@"When O plays in the bottom left");
#line hidden

#line 27
ExecuteLine(@"Then the winner should be O");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Xwins() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 30
ExecuteLine(@"Given the following board:
|X|X|O|
|X|X|O|
|O|O| |");
#line hidden

#line 34
ExecuteLine(@"When X plays in the bottom right");
#line hidden

#line 35
ExecuteLine(@"Then the winner should be X");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void Catsgame() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 38
ExecuteLine(@"Given the following board:
|X|O|X|
|O|O|X|
|X|X|O|");
#line hidden

#line 42
ExecuteLine(@"Then it should be a cat's game");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void horizontalwin() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 45
ExecuteLine(@"Given the following board:
|X|X|X|
|X|O|O|
|O|O|X|");
#line hidden

#line 49
ExecuteLine(@"Then the winner should be X");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void horizontalwininmiddle() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 52
ExecuteLine(@"Given the following board:
|X|O|O|
|X|X|X|
|O|O|X|");
#line hidden

#line 56
ExecuteLine(@"Then the winner should be X");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void horizontalwininbottom() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 59
ExecuteLine(@"Given the following board:
|X|X| |
|X|O|X|
|O|O|O|");
#line hidden

#line 63
ExecuteLine(@"Then the winner should be O");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void verticalwininmiddle() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 66
ExecuteLine(@"Given the following board:
|X|X|O|
|O|X|O|
|O|X|X|");
#line hidden

#line 70
ExecuteLine(@"Then the winner should be X");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void verticalwininright() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 73
ExecuteLine(@"Given the following board:
|X|O|X|
|O|O|X|
|O|X|X|");
#line hidden

#line 77
ExecuteLine(@"Then the winner should be X");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void verticalwininleft() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 80
ExecuteLine(@"Given the following board:
|X|O|O|
|X|O|X|
|X|X|O|");
#line hidden

#line 84
ExecuteLine(@"Then the winner should be X");
#line hidden
  }
  [NUnit.Framework.TestAttribute] public void diagonalwin() {
#line 1  "C:\projects\StorEvil\storevil\Examples\TicTacToe\TicTacToe.feature"
#line hidden
#line 87
ExecuteLine(@"Given the following board:
|X|O|O|
|X|O|X|
|O|X| |");
#line hidden

#line 91
ExecuteLine(@"Then the winner should be O");
#line hidden
  }
  }
}