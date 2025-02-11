using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using NSMB.Utils;


public class UIUpdater : MonoBehaviour {

    public static UIUpdater Instance;
    public GameObject playerTrackTemplate, starTrackTemplate;
    public PlayerController player;
    public Sprite storedItemNull;
    public TMP_Text uiStars, uiCoins, uiDebug, uiLives, uiCountdown, uiLaps;
    public Image itemReserve, itemColor;
    public float pingSample = 0;

    private Material timerMaterial;
    private GameObject starsParent, coinsParent, livesParent, timerParent, lapsParent;
    private readonly List<Image> backgrounds = new();
    private bool uiHidden, invis;

    private int coins = -1, stars = -1, lives = -1, timer = -1, laps = -1;

    public void Start() {
        Instance = this;
        pingSample = PhotonNetwork.GetPing();

        starsParent = uiStars.transform.parent.gameObject;
        coinsParent = uiCoins.transform.parent.gameObject;
        livesParent = uiLives.transform.parent.gameObject;
        timerParent = uiCountdown.transform.parent.gameObject;
        lapsParent = uiLaps.transform.parent.gameObject;

        backgrounds.Add(starsParent.GetComponentInChildren<Image>());
        backgrounds.Add(coinsParent.GetComponentInChildren<Image>());
        backgrounds.Add(livesParent.GetComponentInChildren<Image>());
        backgrounds.Add(timerParent.GetComponentInChildren<Image>());
        backgrounds.Add(lapsParent.GetComponentInChildren<Image>());

        foreach (Image bg in backgrounds)
            bg.color = GameManager.Instance.levelUIColor;
        itemColor.color = new(GameManager.Instance.levelUIColor.r - 0.2f, GameManager.Instance.levelUIColor.g - 0.2f, GameManager.Instance.levelUIColor.b - 0.2f, GameManager.Instance.levelUIColor.a);
    }

    public void Update() {
        pingSample = Mathf.Lerp(pingSample, PhotonNetwork.GetPing(), Mathf.Clamp01(Time.unscaledDeltaTime * 0.5f));
        if (pingSample == float.NaN)
            pingSample = 0;
        
        string signalStrength;
        if (pingSample < 0) {
            signalStrength = "52";
        } else if (pingSample < 80) {
            signalStrength = "49";
        } else if (pingSample < 120) {
            signalStrength = "50";
        } else if (pingSample < 180) {
            signalStrength = "51";
        } else {
            signalStrength = "52";
        }

        uiDebug.text = "<mark=#000000b0 padding=\"20, 20, 20, 20\">" + (int) pingSample + " <sprite=" + signalStrength + ">";

        //Player stuff update.
        if (!player && GameManager.Instance.localPlayer)
            player = GameManager.Instance.localPlayer.GetComponent<PlayerController>();

        if (!player) {
            if (!uiHidden)
                ToggleUI(true);

            return;
        }

        if (uiHidden)
            ToggleUI(false);

        if (player && !invis) UpdateStoredItemUI();
        UpdateTextUI();
    }

    private void ToggleUI(bool hidden) {
        uiHidden = hidden;

        starsParent.SetActive(!hidden);
        livesParent.SetActive(!hidden);
        coinsParent.SetActive(!hidden);
        timerParent.SetActive(!hidden);
        lapsParent.SetActive(!hidden);
    }

    private void UpdateStoredItemUI() {
        if (GameManager.Instance.Togglerizer.currentEffects.Contains("NoReserve"))
        {
            itemReserve.gameObject.SetActive(false);
            invis = true;
        }

        itemReserve.sprite = player.storedPowerup != null ? player.storedPowerup.reserveSprite : storedItemNull;
    }

    private void UpdateTextUI() {
        if (!player || GameManager.Instance.gameover)
            return;
        
        if (GameManager.Instance.starRequirement > 0) {
            if (player.stars != stars) {
                stars = player.stars;
                uiStars.text = Utils.GetSymbolString("Sx" + stars + "/" + GameManager.Instance.starRequirement);
            }
        }
        else starsParent.SetActive(false);
        
        if (GameManager.Instance.raceLevel && GameManager.Instance.lapRequirement > 1) {
            if (player.laps != laps) {
                laps = player.laps;
                uiLaps.text = Utils.GetSymbolString("Lx" + laps + "/" + GameManager.Instance.lapRequirement);
            }
        }
        else lapsParent.SetActive(false);
        
        if (player.coins != coins) {
            coins = player.coins;
            uiCoins.text = Utils.GetSymbolString("Cx" + coins + (GameManager.Instance.coinRequirement > 0 ? "/" + GameManager.Instance.coinRequirement : ""));
        }

        if (player.lives >= 0) {
            if (player.lives != lives) {
                lives = player.lives;
                uiLives.text = Utils.GetCharacterData(player.photonView.Owner).uistring + Utils.GetSymbolString("x" + lives);
            }
        } else {
            livesParent.SetActive(false);
        }

        if (GameManager.Instance.timedGameDuration > 0) {
            int seconds = Mathf.CeilToInt((GameManager.Instance.endServerTime - PhotonNetwork.ServerTimestamp) / 1000f);
            seconds = Mathf.Clamp(seconds, 0, GameManager.Instance.timedGameDuration);
            if (seconds != timer) {
                timer = seconds;
                uiCountdown.text = Utils.GetSymbolString("cx" + (timer / 60) + ":" + (seconds % 60).ToString("00"));
            }
            timerParent.SetActive(true);

            if (GameManager.Instance.endServerTime - PhotonNetwork.ServerTimestamp < 0) {
                if (timerMaterial == null) {
                    CanvasRenderer cr = uiCountdown.transform.GetChild(0).GetComponent<CanvasRenderer>();
                    cr.SetMaterial(timerMaterial = new(cr.GetMaterial()), 0);
                }

                float partialSeconds = (GameManager.Instance.endServerTime - PhotonNetwork.ServerTimestamp) / 1000f % 2f;
                byte gb = (byte) (Mathf.PingPong(partialSeconds, 1f) * 255);
                timerMaterial.SetColor("_Color", new Color32(255, gb, gb, 255));
            }
        } else {
            timerParent.SetActive(false);
        }
    }

    public GameObject CreatePlayerIcon(PlayerController player) {
        GameObject trackObject = Instantiate(playerTrackTemplate, playerTrackTemplate.transform.parent);
        TrackIcon icon = trackObject.GetComponent<TrackIcon>();
        icon.target = player.gameObject;

        trackObject.SetActive(!GameManager.Instance.hideMap);

        return trackObject;
    }
}
