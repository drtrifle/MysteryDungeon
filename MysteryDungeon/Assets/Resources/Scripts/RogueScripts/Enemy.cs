using UnityEngine;
using System.Collections;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MovingObject {
    public int playerDamage;                            //The amount of food points to subtract from the player when attacking.
    public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
    public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.

    private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    private Transform playerTransform;                  //Transform to attempt to move toward each turn.
    private bool skipMove;                              //Boolean to determine whether or not enemy should skip a turn or move this turn.

    private BoardCreator _boardCreator;                 //Reference to BoardCreator class for AStarPathfinding

    private AStar astar;                                //Stores pathfinding solution
    private int astarCounter = 1;                       //Counts the current iteration for pathfinding 
    private bool isPathEstablished = false;             //Flag to check if we should rebuild the AStar path

    private int currentX;                               //Current X world coordinate of enemy
    private int currentY;                               //Current Y world coordinate of enemy

    //Start overrides the virtual Start function of the base class.
    protected override void Start() {
        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        GameManager.instance.AddEnemyToList(this);

        //Get and store a reference to the attached Animator component.
        animator = GetComponent<Animator>();

        //Find the Player GameObject using it's tag and store a reference to its transform component.
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        _boardCreator = GameObject.FindGameObjectWithTag("BoardCreator").GetComponent<BoardCreator>();

        //Call the start function of our base class MovingObject.
        base.Start();
    }


    //Override the AttemptMove function of MovingObject to include functionality needed for Enemy to skip turns.
    //See comments in MovingObject for more on how base AttemptMove function works.
    protected override void AttemptMove<T>(int xDir, int yDir) {
        //Check if skipMove is true, if so set it to false and skip this turn.
        if (skipMove) {
            skipMove = false;
            return;
        }

        //Call the AttemptMove function from MovingObject.
        base.AttemptMove<T>(xDir, yDir);

        //Update Animaions
        animator.SetFloat("moveX", xDir);
        animator.SetFloat("moveY", yDir);
        //Update the LastInput of Player Even if move unsuccessful
        animator.SetFloat("lastMoveX", xDir);
        animator.SetFloat("lastMoveY", yDir);


        //Now that Enemy has moved, set skipMove to true to skip next move.
        //skipMove = true;
    }


    //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
    public void MoveEnemy() {
        //Calculate current position;
        currentX = Mathf.RoundToInt(transform.position.x);
        currentY = Mathf.RoundToInt(transform.position.y);

        //TODO: Check if enemy can see player
        if (isPathEstablished) {
            AStarNode2D nextStep = (AStarNode2D)astar.solution[astarCounter];
            int xDir = nextStep.x - currentX;
            int yDir = nextStep.y - currentY;

            astarCounter++;

            //Current iteration is the final one to get to goal
            if (astarCounter >= astar.solution.Count) {
                isPathEstablished = false;
            }

            AttemptMove<Player>(xDir, yDir);
        } else {
            MoveEnemyToRandom();
        }
    }

    //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
    private void MoveEnemyToRandom() {
        Vector2 targetVector = _boardCreator.GetRandomTileLocation();

        //Prevent rolling the exact same position
        while (targetVector.x == currentX && targetVector.y == currentY) {
            targetVector = _boardCreator.GetRandomTileLocation();
        }

        astar = new AStar(new StoredArrayAStarCost(_boardCreator), currentX, currentY, Mathf.RoundToInt(targetVector.x), Mathf.RoundToInt(targetVector.y));
        astar.findPath();

        //Set flags to tell MoveEnemy() that we successfully computed location 
        isPathEstablished = true;
        astarCounter = 1;

        AStarNode2D nextStep = (AStarNode2D)astar.solution[astarCounter];

        int xDir = nextStep.x - currentX;
        int yDir = nextStep.y - currentY;

        AttemptMove<Player>(xDir, yDir);
    }

    //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
    private void MoveEnemyToPlayer() {
        GetComponent<BoxCollider2D>().enabled = false;
        playerTransform.GetComponent<BoxCollider2D>().enabled = false;

        astar = new AStar(new StoredArrayAStarCost(_boardCreator), currentX, currentY, Mathf.RoundToInt(playerTransform.position.x), Mathf.RoundToInt(playerTransform.position.y));
        astar.findPath();
        AStarNode2D nextStep = (AStarNode2D)astar.solution[1];

        //GetComponent<BoxCollider2D>().enabled = true;
        playerTransform.GetComponent<BoxCollider2D>().enabled = true;

        int xDir = nextStep.x - currentX;
        int yDir = nextStep.y - currentY;

        AttemptMove<Player>(xDir, yDir);
    }


    //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject 
    //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
    protected override void OnCantMove<T>(T component) {
        //Declare hitPlayer and set it to equal the encountered component.
        Player hitPlayer = component as Player;

        //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
        hitPlayer.LoseFood(playerDamage);

        //Set the attack trigger of animator to trigger Enemy attack animation.
        //animator.SetTrigger ("enemyAttack");

        //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
        //SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
    }

    //public method for other classes to damage enemy
    public void TakeDamage(int dmgAmount) {
        //TODO: ADD ENEMY HP

        Destroy(gameObject);

    }
}

