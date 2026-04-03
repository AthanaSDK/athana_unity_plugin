class AthanaSdkResult: Codable {
    let functionName: String
    let data: String?
    let message: String?
    let error: String?

    init(functionName: String, data: String?, message: String?, error: String?) {
        self.functionName = functionName
        self.data = data
        self.message = message
        self.error = error
    }
}