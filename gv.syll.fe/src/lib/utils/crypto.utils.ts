export class CryptoUtils {
	static async generatePKCECodes() {
		const array = new Uint8Array(32);
		crypto.getRandomValues(array);

		// code_verifier
		const codeVerifier = this.base64UrlEncode(array.buffer);

		// code_challenge = SHA256(code_verifier)
		const encoder = new TextEncoder();
		const data = encoder.encode(codeVerifier);
		const digest = await crypto.subtle.digest('SHA-256', data);
		const codeChallenge = this.base64UrlEncode(digest);

		return { codeVerifier, codeChallenge };
	}

     static base64UrlEncode(arrayBuffer: ArrayBuffer) {
        let str = String.fromCharCode(...new Uint8Array(arrayBuffer));
        return btoa(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    }
}
