using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    public int id;
    public int country;

    private static Player player;

    public override void OnStartClient()
    {
        base.OnStartClient();

        id = OwnerId;
        country = OwnerId;

        LobbyManager.instance.AddPlayer(this);
        transform.parent = LobbyManager.instance.transform;

        if (!IsOwner)
        {
            return;
        }

        Texture2D flagTexture = CountryLoader.countries[country].GetFlag();
        Sprite sprite = Sprite.Create(flagTexture, new Rect(0, 0, flagTexture.width, flagTexture.height), new Vector2(0.5f, 0.5f));

        TheGameManager.instance.flag.sprite = sprite;
        player = this;
    }

    public static Player GetPlayer()
    {
        return player;
    }

}
