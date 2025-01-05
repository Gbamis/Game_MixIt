using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Coin", menuName = "Games/Wine/Coin")]
public class Coin : ScriptableObject
{
    public int account_balance;

    public Action<int, Vector2> OnAccountChanged;

    public void Credit(int amount, Vector2 pos)
    {
        account_balance += amount;
        OnAccountChanged?.Invoke(amount, pos);
    }

    public void CreditFake(int amount, Vector2 pos) => OnAccountChanged?.Invoke(amount, pos);


    public bool Debit(int amount)
    {
        if (account_balance >= amount)
        {
            account_balance -= amount;
            OnAccountChanged?.Invoke(-amount, Vector2.zero);
            return true;
        }
        return false;
    }

    public bool CanDebit(int amount) => account_balance > amount;
}
