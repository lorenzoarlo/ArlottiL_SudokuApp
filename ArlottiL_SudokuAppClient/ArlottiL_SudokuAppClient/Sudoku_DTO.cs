namespace ArlottiL_SudokuAppClient
{
    public class Sudoku_DTO
    {
        public int ID { get; set; }

        public string Mission { get; set; }

        public string Solution { get; set; }

        public Sudoku_Board.Difficulty Difficulty { get; set; }
    }
}
