using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool grounded = false;
    private MovimentControler jump;

    void Start()
    {
        // Guarda uma referência pro JumpController, porque vamos usar ele com frequência
        jump = GetComponentInParent<MovimentControler>();
    }

    // Se entrou em algum colisor, considera como chão
    // A fazer: filtrar por layers
    void OnTriggerEnter2D(Collider2D collider)
    {
        grounded = true;
        
        jump.Ground();
    }

    // Ao sair do colisor, precisamos checar se algum objeto ainda tá dentro dele
    void OnTriggerExit2D()
    {
        // Só faz o check de sair do chão se o player estiver no chão
        if (grounded)
        {
            // ContactFilter -> usado pra selecionar o que vai ser considerado nesse check
            // No caso, tudo da layer "Default". O player e os checks tão na layer "Player"
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));

            // Array de colisores, que vai ser preenchido pela função "OverlapCollider"
            // Ele tem tamanho 1 porque só queremos descobrir se o resultado é 0 ou não, 
            // e o tamanho desse array é o máximo de colisores que o OverlapCollider retorna
            Collider2D[] results = new Collider2D[1];

            // Função que retorna quantos colisores estão dentro desse
            // Se nenhum outro colisor fizer overlap com esse, o player não tá mais no chão.
            if (GetComponent<BoxCollider2D>().OverlapCollider(contactFilter, results) == 0)
                grounded = false;
        }
    }

    // O parâmetro "grounded" é privado, pra que somente o GroundCheck possa mudar seu valor
    // Mas temos um método público, pra ler o valor
    public bool isGrounded()
    {
        return grounded;
    }
}
