window.cryptoFunctions = {
    encrypt: async (data, password) => {
        try {
            // Generate a random salt for the key derivation
            const salt = window.crypto.getRandomValues(new Uint8Array(16));
            
            // Derive a key from the password using PBKDF2
            const keyMaterial = await window.crypto.subtle.importKey(
                "raw",
                new TextEncoder().encode(password),
                { name: "PBKDF2" },
                false,
                ["deriveBits", "deriveKey"]
            );
            const key = await window.crypto.subtle.deriveKey(
                {
                    "name": "PBKDF2",
                    salt: salt,
                    "iterations": 100000,
                    "hash": "SHA-256"
                },
                keyMaterial,
                { "name": "AES-GCM", "length": 256 },
                true,
                ["encrypt", "decrypt"]
            );

            // Generate a random Initialization Vector (IV)
            const iv = window.crypto.getRandomValues(new Uint8Array(12));
            
            // Encrypt the data
            const encryptedContent = await window.crypto.subtle.encrypt(
                {
                    name: "AES-GCM",
                    iv: iv
                },
                key,
                data
            );
            
            // Combine salt, IV, and encrypted data into a single buffer
            // Format: [16 bytes salt][12 bytes IV][encrypted data]
            const encryptedData = new Uint8Array(salt.length + iv.length + encryptedContent.byteLength);
            encryptedData.set(salt, 0);
            encryptedData.set(iv, salt.length);
            encryptedData.set(new Uint8Array(encryptedContent), salt.length + iv.length);

            // Return as a Base64 string for easy handling in C#
            return btoa(String.fromCharCode.apply(null, encryptedData));
        } catch (error) {
            console.error("Encryption failed:", error);
            return null;
        }
    },
    decrypt: async (encryptedBase64, password) => {
        try {
            // Decode the Base64 string back to a byte array
            const encryptedData = new Uint8Array(atob(encryptedBase64).split("").map(c => c.charCodeAt(0)));

            // Extract salt, IV, and the actual encrypted content
            const salt = encryptedData.slice(0, 16);
            const iv = encryptedData.slice(16, 28);
            const data = encryptedData.slice(28);

            // Derive the key from the password using the same salt and parameters
            const keyMaterial = await window.crypto.subtle.importKey(
                "raw",
                new TextEncoder().encode(password),
                { name: "PBKDF2" },
                false,
                ["deriveBits", "deriveKey"]
            );
            const key = await window.crypto.subtle.deriveKey(
                {
                    "name": "PBKDF2",
                    salt: salt,
                    "iterations": 100000,
                    "hash": "SHA-256"
                },
                keyMaterial,
                { "name": "AES-GCM", "length": 256 },
                true,
                ["encrypt", "decrypt"]
            );

            // Decrypt the data
            const decryptedContent = await window.crypto.subtle.decrypt(
                {
                    name: "AES-GCM",
                    iv: iv
                },
                key,
                data
            );
            
            // The result is an ArrayBuffer. We don't convert it to a string here
            // because the C# side will handle the byte array.
            return new Uint8Array(decryptedContent);
        } catch (error) {
            console.error("Decryption failed:", error);
            // This can happen if the password is wrong or the data is corrupt
            return null;
        }
    }
}; 