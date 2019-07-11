using System;

namespace Mergen.Core.EntityIds
{
    [Flags]
    public enum GameStateIds
    {
        SelectCategory = 1,
        Player1AnswerQuestions = 2,
        Player2AnswerQuestions = 3,
        Completed = 4
    }
}