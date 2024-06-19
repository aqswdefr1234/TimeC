using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobForward
{

#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9996067309099965/4186585920";//실제 광고 id:ca-app-pub-9996067309099965/4186585920, 테스트용 : ca-app-pub-3940256099942544/1033173712
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
  private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;
    bool isInitialized = false;

    public static AdMobForward Instance 
    {
        get
        {
            if (instance == null) 
            {
                instance = new AdMobForward();
            }
            return instance;
        }
    }
    public static AdMobForward instance = null;
    public void StartInitialize()
    {
        if (isInitialized) return;
        isInitialized = true;
        //앱 실행시 한번만 초기화 하면됨
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });
        //씬 시작되면 미리 로드. 수명은 1시간
        LoadInterstitialAd();
    }
    //Loads the interstitial ad
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }
                _interstitialAd = ad;
                RegisterReloadHandler(_interstitialAd);
            });
    }

    //Shows the interstitial ad.
    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _interstitialAd.Show();
        }
    }

    void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }
}