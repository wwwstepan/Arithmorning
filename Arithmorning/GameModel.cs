using CommunityToolkit.Mvvm.ComponentModel;
using Timer = System.Timers.Timer;

namespace Arithmorning;

public partial class GameModel : ObservableObject
{
    private readonly Random rnd = new();
    private readonly Timer timer;

    [ObservableProperty]
    private int _scoresToWriteAnswer = 0;

    [ObservableProperty]
    private int _countDown;

    [ObservableProperty]
    private bool _gameIsActive = false;

    [ObservableProperty]
    private int _scores = 0;

    [ObservableProperty]
    private string _challenge = string.Empty;

    private int WriteAnswer = 0;

    public GameModel()
    {
        timer = new Timer(1000);
        timer.Elapsed += TimerElapsed;
    }

    private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (CountDown <= 1)
        {
            StopGame();
            return;
        }

        CountDown--;
        timer.Start();
    }

    public bool ProcessAnswer(string answer, out int writeAnswer)
    {
        var res = false;
        writeAnswer = WriteAnswer;

        if (int.TryParse(answer, out int numAnswer) && numAnswer == writeAnswer)
        {
            res = true;
            Scores += ScoresToWriteAnswer;
        }

        GenerateChallenge();

        return res;
    }

    private void GenerateChallenge()
    {        
        int arg1 = rnd.Next(11, 99);
        int arg2 = rnd.Next(11, 99);

        var oper = rnd.Next(0, 7) switch {
            0  => '+',
            1 => '-',
            2 or 3 => '/',
            _ => 'x' // 4 or 5 or 6
        };

        if (oper == '-' && arg2 > arg1)
        {
            (arg1, arg2) = (arg2, arg1);
        }
        else if (oper == '/')
        {
            arg2 = rnd.Next(3, 20);
            arg1 = rnd.Next(11, 30) * arg2;
        }

        (WriteAnswer, ScoresToWriteAnswer) = oper switch {
            '+' => (arg1 + arg2, 1),
            '-' => (arg1 - arg2, 1),
            '/' => (arg1 / arg2, 4),
            'x' => (arg1 * arg2, 7),
            _ => (0, 0)
        };

        if (oper == '/' && arg2 > 10)
            ScoresToWriteAnswer += 2;
        else if (oper == 'x')
        {
            var dig11 = arg1 / 10;
            var dig21 = arg2 / 10;
            var dig12 = arg1 % 10;
            var dig22 = arg2 % 10;

            if (
                (dig11 > 2 && dig11 < 8)
                || (dig21 > 2 && dig21 < 8)
                || (dig12 > 2 && dig12 < 8)
                || (dig22 > 2 && dig22 < 8)
            )
                ScoresToWriteAnswer += 3;
        }

        Challenge = $"{arg1} {oper} {arg2}";
    }

    public void StartGame()
    {
        Scores = 0;
        
        GenerateChallenge();
        
        GameIsActive = true;

        CountDown = 100;
        timer.Start();
    }

    public void StopGame()
    {
        timer.Stop();
        GameIsActive = false;
    }

    public void AddScores(int add)
    {
        if (add != 0)
            Scores += add;
    }
}