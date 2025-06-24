// This script provides JavaScript interop functions for the File System Access API.

// A global variable to hold the file handle. This allows us to save back to the same file
// without asking the user to select it again.
let fileHandle;

/**
 * Opens a file picker dialog for the user to select a '.db' file.
 * If a file is selected, its handle is stored, and its contents are read as a byte array.
 * @returns {Uint8Array} The contents of the selected file as a byte array, or null if no file was selected.
 */
async function openFile() {
    try {
        [fileHandle] = await window.showOpenFilePicker({
            types: [{
                description: 'Knowledge Hub Database',
                accept: { 'application/octet-stream': ['.db'] }
            }],
        });

        if (!fileHandle) {
            return null;
        }

        const file = await fileHandle.getFile();
        const buffer = await file.arrayBuffer();
        return new Uint8Array(buffer);
    } catch (error) {
        // Log errors, but don't bubble them up to the C# layer as exceptions
        // Often this is just the user closing the dialog
        console.error("Error opening file:", error);
        return null;
    }
};

/**
 * Saves the provided data to the currently held file handle.
 * If no file handle is held, it will call the saveAs function.
 * @param {Uint8Array} data The byte array data to save.
 * @returns {boolean} True if the save was successful, false otherwise.
 */
async function saveFile(data) {
    if (!fileHandle) {
        return await saveFileAs(data);
    }
    try {
        const writable = await fileHandle.createWritable();
        await writable.write(data);
        await writable.close();
        return true;
    } catch (error) {
        console.error("Error saving file:", error);
        return false;
    }
};

/**
 * Opens a 'Save As' dialog for the user to choose a location and name for a new '.db' file.
 * It then saves the provided data to that new file.
 * @param {Uint8Array} data The byte array data to save.
 * @returns {boolean} True if the save was successful, false otherwise.
 */
async function saveFileAs(data) {
    try {
        const options = {
            types: [{
                description: 'Knowledge Hub Database',
                accept: { 'application/octet-stream': ['.db'] }
            }],
        };
        fileHandle = await window.showSaveFilePicker(options);

        const writable = await fileHandle.createWritable();
        await writable.write(data);
        await writable.close();
        return true;
    } catch (error) {
        console.error("Error saving file as:", error);
        return false;
    }
}; 