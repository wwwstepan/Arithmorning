using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Timer = System.Timers.Timer;

namespace Arithmorning;

public partial class MainPageViewModel : ObservableObject
{
    private readonly GameModel _gameModel = new();
    private readonly Timer timerWriteAnswer;

    [ObservableProperty]
    private bool _gameIsActive = false;

    public bool GameIsNotActive => !GameIsActive;

    [ObservableProperty]
    private int _countDown;

    [ObservableProperty]
    private string _challenge = string.Empty;

    [ObservableProperty]
    private int _scores = 0;

    [ObservableProperty]
    private string _answer = string.Empty;

    [ObservableProperty]
    private bool _showWriteAnswer = false;

    [ObservableProperty]
    private int _writeAnswer = 0;

    [ObservableProperty]
    private int _scoresToWriteAnswer = 0;

    public MainPageViewModel()
    {
        _gameModel.PropertyChanged += GameModelPropertyChanged;
        timerWriteAnswer = new Timer(3000);
        timerWriteAnswer.Elapsed += TimerWriteAnswerElapsed;
    }

    private void TimerWriteAnswerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        timerWriteAnswer.Stop();
        ShowWriteAnswer = false;
    }

    [RelayCommand]
    public void StartGame()
    {
        _gameModel.StartGame();

        Challenge = _gameModel.Challenge;
        CountDown = 60;
        Answer = string.Empty;
    }

    [RelayCommand]
    public void ProcessAnswer()
    {
        int writeAnswer = 0;
        var res = _gameModel?.ProcessAnswer(Answer, out writeAnswer) ?? false;
        Answer = string.Empty;

        if (!res)
        {
            WriteAnswer = writeAnswer;
            ShowWriteAnswer = true;
            timerWriteAnswer.Start();
        }
    }

    [RelayCommand]
    public void AddAnswerSymbol(string symbol) => Answer += symbol;

    [RelayCommand]
    public void ClearAnswer() => Answer = string.Empty;

    private void GameModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs? e)
    {
        switch(e?.PropertyName)
        {
            case "GameIsActive":
                GameIsActive = _gameModel.GameIsActive;
                break;
            case "CountDown":
                CountDown = _gameModel.CountDown;
                break;
            case "Challenge":
                Challenge = _gameModel.Challenge;
                break;
            case "Scores":
                Scores = _gameModel.Scores;
                break;
            case "ScoresToWriteAnswer":
                ScoresToWriteAnswer = _gameModel.ScoresToWriteAnswer;
                break;
            default: break;
        }
    }
}
