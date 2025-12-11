public class Player_OnInteract_ReturnTool : Player_OnEvent_Behaviour
{
    public override void OnEventBehave()
    {
        player.ReturnTool();
        player.EndToolScan();
    }
}