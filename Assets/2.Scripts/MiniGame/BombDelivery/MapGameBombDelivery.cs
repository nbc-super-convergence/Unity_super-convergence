using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGameBombDelivery : MapBase
{
    private Bomb bomb;
    private List<MiniToken> tokens;

    Dictionary<string, int> rank = new();

    private int target;

    private void Awake()
    {
        MinigameManager.Instance.SetMiniGame<GameBombDelivery>();
    }

    public async void Init()
    {
        GameObject prefab = await ResourceManager.Instance.LoadAsset<GameObject>("bomb", eAddressableType.Prefab);
        bomb = Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<Bomb>();
        bomb.gameObject.SetActive(false);

        tokens = MinigameManager.Instance.miniTokens.ToList();
    }

    public void PlayerOut()
    {
        bomb.gameObject.SetActive(false);
        var p = tokens[target];

        //p.MiniData.
        //rank[] = tokens.Count;
        tokens.RemoveAt(target);

        p.DisableMiniToken();

        if (tokens.Count == 1)
        {
            //MinigameManager.Instance.GetMiniGame<GameBombDelivery>().GameEnd(rank);
        }
        else
            SetBomb();
    }

    public void ChangeTarget(MiniToken m)
    {
        target = tokens.IndexOf(m);
        bomb.SetTarget(m.transform,target);
    }

    public void SetBomb()
    {
        target = Random.Range(0, tokens.Count);
        bomb.SetTarget(tokens[target].transform,target);

        bomb.gameObject.SetActive(true);
    }
}