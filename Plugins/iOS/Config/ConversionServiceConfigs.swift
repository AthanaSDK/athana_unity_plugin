class ConversionServiceConfigs: Codable {
    
    let appsflyer: AppsflyerConversionServiceConfig?

    init (appsflyer: AppsflyerConversionServiceConfig? = nil) {
        self.appsflyer = appsflyer
    }
}

class AppsflyerConversionServiceConfig: Codable {
    let sdkKey: String
    let appId: String
    let manualStart: Bool

    init (
        sdkKey: String,
        appId: String,
        manualStart: Bool = false
    ) {
        self.sdkKey = sdkKey
        self.appId = appId
        self.manualStart = manualStart
    }
}