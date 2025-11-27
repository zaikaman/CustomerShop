// Local Storage Helper cho Blazor Server
window.localStorageHelper = {
    setItem: function (key, value) {
        try {
            localStorage.setItem(key, value);
            return true;
        } catch (e) {
            console.error('Error saving to localStorage:', e);
            return false;
        }
    },
    
    getItem: function (key) {
        try {
            return localStorage.getItem(key);
        } catch (e) {
            console.error('Error reading from localStorage:', e);
            return null;
        }
    },
    
    removeItem: function (key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (e) {
            console.error('Error removing from localStorage:', e);
            return false;
        }
    },
    
    clear: function () {
        try {
            localStorage.clear();
            return true;
        } catch (e) {
            console.error('Error clearing localStorage:', e);
            return false;
        }
    }
};
