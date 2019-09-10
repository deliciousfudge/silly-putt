using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class S_IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController ms_StoreController;
    private static IExtensionProvider ms_StoreExtensionProvider;

    public static string ms_kRemoveAds = "removeads";

    // Checks to see if the app can connect to Unity IAP
    // Will continue trying in the background until successfully connected
    public void OnInitialized(IStoreController _Controller, IExtensionProvider _Extensions)
    {
        print("OnInitialized: Pass");
        ms_StoreController = _Controller;
        ms_StoreExtensionProvider = _Extensions;
    }

    // Called when IAP have failed to initialize and provides the failure reason
    public void OnInitializeFailed(InitializationFailureReason _Error)
    {
        print("OnInitializeFailed: " + _Error);
    }

    // Checks if both purchasing references are set
    private bool IsInitialized()
    {
        return ms_StoreController != null && ms_StoreExtensionProvider != null;
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        // Collects products and store-specific configuration details
        var Builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product with a Unity IAP ID
        Builder.AddProduct(ms_kRemoveAds, ProductType.Consumable);

        // Intialize Unity IAP with the specified listener and configuration
        // Store Controller and Extension Provider are set
        UnityPurchasing.Initialize(this, Builder);
    }

    // Handles the purchase of a product
    public void BuyProductID(string _ProductID)
    {
        if (IsInitialized())
        {
            Product ProductInstance = ms_StoreController.products.WithID(_ProductID);
            if (ProductInstance != null && ProductInstance.availableToPurchase)
            {
                print("BuyProductID: Purchasing product");
                ms_StoreController.InitiatePurchase(ProductInstance);
            }
            else
            {
                print("BuyProductID: Product either is not found or not available for purchase");
            }
        }
        else
        {
            print("BuyProductID: Failure. Not initialized");
        }
    }

    // Performs any necessary adjustments when a consumable product has been purchased
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs _Args)
    {
        // If the player has purchased the option to remove ads
        if (string.Equals(_Args.purchasedProduct.definition.id, ms_kRemoveAds, System.StringComparison.Ordinal))
        {
            print("ProcessPurchase: Purchasing Product");
            PlayerPrefs.SetString("ShouldGameDisplayAds", "No");
        }
        else
        {
            print("ProcessPurchase: Unrecognized Product");
        }

        return PurchaseProcessingResult.Complete;
    }

    // Logs a message to the console telling us when a purchase failed
    public void OnPurchaseFailed(Product _Product, PurchaseFailureReason _FailureReason)
    {
        print(string.Format("OnPurchaseFailed: Purchase of '{0}' failed due to {1}", _Product.definition.storeSpecificId, _FailureReason));
    }

    // Start is called before the first frame update
    void Start()
    {
        if (ms_StoreController == null)
        {
            // Configure our connection to purchasing
            InitializePurchasing();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
