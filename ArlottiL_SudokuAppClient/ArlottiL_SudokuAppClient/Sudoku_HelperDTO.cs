using System.Collections.Generic;

namespace ArlottiL_SudokuAppClient
{
    public class Sudoku_HelperDTO
    {
        public string CandidateString { get; set; }
        public IEnumerable<Sudoku_ActionDTO> Actions { get; set; }
    }


}