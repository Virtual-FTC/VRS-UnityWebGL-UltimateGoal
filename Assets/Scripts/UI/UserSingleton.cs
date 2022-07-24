using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSingleton : MonoBehaviour
{
    public static UserSingleton instance;
    [SerializeField] public User localUserType = User.student;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);
    }
}

public enum User
{
    supervisor,
    student
}
