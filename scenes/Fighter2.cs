using Godot;

// Inheritance: Fighter2 is a Fighter, so it keeps movement, attacking and health.
// Polymorphism: it only overrides which controls and animations it uses.
public partial class Fighter2 : Fighter
{
	protected override string InputPrefix => "p2";
	protected override string Attack1Action => "p2_attack";
	protected override string Attack2Action => "p2_attack2";

	protected override string IdleAnim => "p2idle";
	protected override string RunAnim => "p2run";
	protected override string JumpAnim => "p2jump";
	protected override string FallAnim => "p2fall";

	protected override string Attack1Anim => "p2attack1";
	protected override string Attack2Anim => "p2attack2";
}
