<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
using JetBrains.Annotations;
>>>>>>> Stashed changes
=======
using JetBrains.Annotations;
>>>>>>> Stashed changes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
<<<<<<< Updated upstream

[System.Serializable]
public class EnemyData
=======
[System.Serializable]
public class EnemyData 
>>>>>>> Stashed changes
=======
[System.Serializable]
public class EnemyData 
>>>>>>> Stashed changes
{
    public enum EnemyClass
    {
        Enforcer,
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    }
=======
        Warden,
    }

>>>>>>> Stashed changes
=======
        Warden,
    }

>>>>>>> Stashed changes
    public enum PersionalityType
    {
        Aggressive,
    }

    public string enemyID;
    public string enemyName;
    public EnemyClass enemyClass;
    public PersionalityType persionalityType;
    public int health;
    public float moveSpeed;
    public int resistance;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public float attackPower;
=======
    public int attackPower;
>>>>>>> Stashed changes
=======
    public int attackPower;
>>>>>>> Stashed changes
    public float attackSpeed;
    public int experienceGiven;
    public string dropTalbeID;
    public string visualResourceID;
    public string soundResourceID_Attack;
    public string soundResourceID_Hit;
    public string soundResourceID_Death;
    public string notes;
<<<<<<< Updated upstream
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
}

[System.Serializable]
public class EnemyDataSheet
{
    public EnemyData[] enemyDataSheet;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
=======
}
>>>>>>> Stashed changes
