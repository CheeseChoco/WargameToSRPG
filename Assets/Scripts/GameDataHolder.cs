// GameDataHolder.cs

// static 클래스는 게임 내에 단 하나만 존재하며, 어디서든 접근할 수 있습니다.
public static class GameDataHolder
{
    // 여기에 저장된 데이터는 씬이 바뀌어도 절대 사라지지 않습니다.
    public static int SelectedStageNum { get; set; }
}