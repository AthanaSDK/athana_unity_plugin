import AthanaCore

class AdServiceConfigs: Codable {

    let max: MaxAdServiceConfig?

    init ( max: MaxAdServiceConfig? = nil) {
        self.max = max
    }

}

class MaxAdServiceConfig: Codable {
    let sdkKey: String
    let privacyPolicyUrl: String?
    let termsOfServiceUrl: String?
    let debug: Bool
    let preloadAds: String?
    let autoLoadNext: Bool
    
    var typedPreloadAds: [Int: String]? {
        guard let data = preloadAds?.data(using: .utf8),
              let rawDict = try? JSONDecoder().decode([String: String].self, from: data) else {
            return nil
        }

        var result = [Int: String]()
        for (key, value) in rawDict {
            result[key.toAdType().rawValue] = value
        }
        return result.isEmpty ? nil : result
    }

    init (
        sdkKey: String,
        privacyPolicyUrl: String? = nil,
        termsOfServiceUrl: String? = nil,
        debug: Bool = false,
        preloadAds: String? = nil,
        autoLoadNext: Bool = true
    ) {
        self.sdkKey = sdkKey
        self.privacyPolicyUrl = privacyPolicyUrl
        self.termsOfServiceUrl = termsOfServiceUrl
        self.debug = debug
        self.preloadAds = preloadAds
        self.autoLoadNext = autoLoadNext
    }
}
