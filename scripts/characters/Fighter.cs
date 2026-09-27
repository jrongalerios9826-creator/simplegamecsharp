using Godot;

// Inheritance: Fighter gets Speed, Gravity, Health, Move(), TakeDamage()
// all for free from Character, and adds punching on top
public partial class Fighter : Character
{
	private bool _isAttacking = false;
	private float _attackTimer = 0f;
	private const float AttackDuration = 0.25f;
	private string _currentAttackAnim = "attack1";

	private Area2D _attackArea;

	public override void _Ready()
	{
		base._Ready();

		_attackArea = GetNode<Area2D>("AttackArea");
		_attackArea.Monitoring = false;
		_attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y += Gravity * dt;

		float moveInput = 0f;
		if (Input.IsActionPressed("p1_left")) moveInput -= 1;
		if (Input.IsActionPressed("p1_right")) moveInput += 1;

		bool jumpPressed = Input.IsActionJustPressed("p1_jump");
		bool punchPressed = Input.IsActionJustPressed("p1_punch");
		bool punch2Pressed = Input.IsActionJustPressed("p1_punch2");

		if (!_isAttacking)
		{
			Move(moveInput, jumpPressed, ref velocity);

			if (punchPressed)
				StartAttack("attack1");
			else if (punch2Pressed)
				StartAttack("attack2");
		}
		else
		{
			velocity.X = 0;
		}

		Velocity = velocity;
		MoveAndSlide();

		if (_isAttacking)
		{
			_attackTimer -= dt;
			if (_attackTimer <= 0)
				EndAttack();
		}
	}

	private void StartAttack(string animationName)
	{
		_isAttacking = true;
		_attackTimer = AttackDuration;
		_currentAttackAnim = animationName;

		_attackArea.Monitoring = true;
		_attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;

		var shape = _attackArea.GetNode<CollisionShape2D>("CollisionShape2D");
		shape.Position = new Vector2(_facingRight ? 30 : -30, 0);

		_sprite.Play(_currentAttackAnim);
		_sprite.Modulate = Colors.Red;
	}

	private void EndAttack()
	{
		_isAttacking = false;
		_attackArea.Monitoring = false;
		_attackArea.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;

		_sprite.Modulate = Colors.White;
	}

	public void OnAttackAreaBodyEntered(Node2D body)
	{
		if (body is Fighter other && other != this && _isAttacking)
		{
			other.TakeDamage(10);
		}
	}

	public override void TakeDamage(int amount)
	{
		base.TakeDamage(amount);
		_sprite.Modulate = Colors.Red;
	}
}
