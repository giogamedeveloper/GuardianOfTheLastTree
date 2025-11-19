using UnityEngine;

//Esta es una interfaz que nos permitirá obligar a incorporar ciertos métodos a las clases que la implementen.
//Está definida como genérica donde T es un "placeholder" de un tipo de dato que será definido en la clase en que se implemente la interfaz.
public interface IDamageable<T>
{
    bool isDead();
    //La clase que implememte esta interfaz estará obligada a definir e implementar este método que usará el tipo indicado como primer parámetro y un vector 3 para indicar el pinto de impacto. 
    //Se usa default para obtener el valor predeterminado de un tipo concreto. Al asignar un valor a un paramtero estamos haciendo que este sea opcional.
    void TakeDamage(T damage, Vector3 impactPosition = default(Vector3));
}
