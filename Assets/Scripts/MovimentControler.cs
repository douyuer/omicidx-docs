using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MovimentControler : MonoBehaviour
{
    /* Mecânica de Pulo */

    // Ângulo

    public float maxAngle = 40;
    public float angleSpeed = 2;

    private float angleTracker = 0;
    private bool angleDir = true;

    // Força

    public float maxForce = 10;
    public float forceSpeed = 10;

    private float forceTracker = 0;
    private bool forceDir = true;

    /* Estado de Jogo do movimento */

    // Estágio do controlador (0: escolhendo ângulo, 1: escolhendo força, 2: pulando)
    public int stage = 0;

    // Stamina do player
    private float stamina = 1.0f;

    //Distancia do raycast
    public float raycast_length = 2.0f;

    //Grounded
    private bool grounded = false;

    /* Referências para outros componentes */

    private SpriteRenderer sprite;
    new private Rigidbody2D rigidbody;

    /* Parâmetros da Stamina */

    public float InitialStaminaCost = 0.3f;
    public float StaminaCost = 0.2f;
    public float StaminaRecoverRate = 0.5f;

    /* Start
     * Inicializa os trackers e as referências pra outros componentes */
    void Start()
    {
        angleTracker = 0;
        forceTracker = maxForce;

        sprite = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    /* FixedUpdate -> Interação física usa o FixedUpdate, não o Update
     * Coração do script de pulo do jogo */
    void Update()
    {
        // checar se esta no chão
        Vector2 raycast_from = (Vector2)this.transform.position + new Vector2(0, -sprite.bounds.size.y / 2);
        RaycastHit2D hit = Physics2D.Raycast(raycast_from, Vector2.down, raycast_length);

        if (hit.collider == null) 
        {
            grounded = false;
            Debug.Log("nao chão");

        }
        else
        { if (grounded == false) Ground();
            grounded = true;
            

        }
            


        // Estágio 0: escolhendo ângulo
        if (stage == 0)
        {
            // Caso a stamina não esteja cheia, recupera de acordo com o deltaTime
            if (stamina < 1)
            {
                stamina += Time.deltaTime * StaminaRecoverRate;
                if (stamina > 1) stamina = 1;
            }

            // Lê o input das setas, pra virar o player
            // A variável "flipX" do SpriteRenderer é a única que guarda
            // a informação de qual o lado para onde o player está olhando.
            if (Input.GetKeyDown(KeyCode.LeftArrow)) sprite.flipX = true;
            if (Input.GetKeyDown(KeyCode.RightArrow)) sprite.flipX = false;

            // Direção true -> ângulo subindo
            if (angleDir)
            {
                // Incrementa o tracker e inverte se chegar no máximo
                angleTracker += angleSpeed * Time.deltaTime;
                if (angleTracker >= maxAngle)
                {
                    angleTracker = maxAngle;
                    angleDir = false;
                }
            }
            // Direção false -> ângulo descendo
            else
            {
                // Decrementa o tracker e inverte se chegar no mínimo
                angleTracker -= angleSpeed * Time.deltaTime;
                if (angleTracker <= 0)
                {
                    angleTracker = 0;
                    angleDir = true;
                }
            }

            // Caso o player aperte espaço nesse estágio, avança para o próximo
            if (Input.GetKeyDown(KeyCode.Space))
            {
                stage = 1;
                
            }
        }
        // Estágio 1: escolhendo força
        else if (stage == 1)
        {
            // Direção true -> força crescendo
            if (forceDir)
            {
                // Incrementa o tracker e inverte se chegar no máximo
                forceTracker += forceSpeed * Time.deltaTime;
                if (forceTracker >= maxForce)
                {
                    forceTracker = maxForce;
                    forceDir = false;
                }
            }
            // Direção true -> força diminuindo
            else
            {
                // Decrementa o tracker e inverte se chegar no mínimo
                forceTracker -= forceSpeed * Time.deltaTime;
                if (forceTracker <= 0)
                {
                    forceTracker = 0;
                    forceDir = true;
                }
            }

            // Caso o player aperte espaço nesse estágio, pula e avança para o próximo
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // O pulo gasta uma quantidade inicial de stamina
                // A fazer: não pular se a stamina for menor do que essa quantidade
                Jump(InitialStaminaCost);


                // Vai para o estágio 2: pulando
                stage = 2;
                
            }
            
            

            
        }
    }

    /* Jump
     * Faz o player pular na direção do Sprite, com a força e ângulo sorteados (os trackers)
     * A força é multiplicada pela stamina, e a stamina é reduzida com base no custo */
    void Jump(float stamina_cost)
    {
        // Angulo sorteado (graus -> radianos)
        float angle = Mathf.Deg2Rad * angleTracker;
        // Força sorteada multiplicada pela stamina
        float force = forceTracker * stamina;
        // Direção do pulo em X (-1 = esquerda, 1 = direita)
        float x_dir = sprite.flipX ? -1 : 1;

        //Debug.Log(new Vector2(x_dir * Mathf.Cos(angle), Mathf.Sin(angle)));

        // Transforma o ângulo em um vetor com sin/cos,
        // inverte em X se necessário,
        // multiplica o vetor (de tamanho 1) pela força e aplica como Impulso
        rigidbody.AddForce(new Vector2(x_dir * Mathf.Cos(angle), Mathf.Sin(angle)) * force, ForceMode2D.Impulse);
        // Reduz a stamina com base no custo pra esse pulo (recebido como parâmetro na função)
        stamina -= stamina_cost;

        //marcar grounded como falso
        grounded = false;
    }

    /* Reflect
     * Chamado pelo WallCheck quando o player bate de lado numa parede */
    public void Reflect()
    {
        // Inverte o sprite
        // A velocidade do rigidbody quando essa função é chamada ainda é a
        // velocidade antes da colisão. Então, se for maior ou igual a 0, o 
        // player deveria virar pra esquerda.
        sprite.flipX = rigidbody.velocity.x >= 0;

        // Define a velocidade do objeto como zero (pensa nisso, o gato em algum frame
        // no meio do movimento tá absolutamente parado)
        rigidbody.velocity = Vector2.zero;

        // Pula denovo na direção contrária (definida no sprite.flipX)
        // O custo de stamina aqui é diferente do inicial
        Jump(StaminaCost);
    }

    /* Ground
     * Chamado pelo GroundCheck quando o player encosta no chão 
     * Reseta os trackers e volta pro estágio 0*/
    public void Ground()
    {
        angleTracker = 0;
        forceTracker = maxForce;

        stage = 0;
    }

    /* Desenha os Gizmos */
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Vetor de força do pulo
        float angle = Mathf.Deg2Rad * angleTracker;
        float force = forceTracker / 10.0f;
        float x_dir = (sprite != null && sprite.flipX) ? -1 : 1;
        Gizmos.DrawLine(this.transform.position, (Vector2)this.transform.position + new Vector2(x_dir * Mathf.Cos(angle) * force, Mathf.Sin(angle) * force));

        // Informações textuais no player
        UnityEditor.Handles.Label(transform.position + new Vector3(-0.5f, 1.0f, 0), "stage: " + stage.ToString() + "\nstamina: " + stamina.ToString());

        //raycast do chão
        Gizmos.color = Color.green;
        SpriteRenderer spritea = GetComponent<SpriteRenderer>();
        Vector2 raycast_from = (Vector2)this.transform.position + new Vector2(0, -spritea.bounds.size.y/2);
        Vector2 raycast_to = raycast_from + new Vector2(0, - raycast_length);
        Gizmos.DrawLine(raycast_from, raycast_to);
        

    }

    /* Getters*/
    public int getStage()
    {
        return stage;
    }

    public bool getGrounded()
    {
        return grounded;
    }

}
