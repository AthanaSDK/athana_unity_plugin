import AthanaCore

class AccountServiceConfig: Codable {

    let enabledSignInTypes: [SignInType]?

    init(enabledSignInTypes: [SignInType]?) {
        self.enabledSignInTypes = enabledSignInTypes
    }

}