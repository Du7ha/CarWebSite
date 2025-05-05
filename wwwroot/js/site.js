// Add this to your site.js file or include directly in the layout

document.addEventListener('DOMContentLoaded', function() {
    // Get references to the sign-in form elements
    const signInForm = document.querySelector('form[action*="SignIn"]');
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');
    const signInButton = signInForm?.querySelector('button[type="submit"]');
    
    // Create validation message element
    const validationMessage = document.createElement('div');
    validationMessage.className = 'text-danger mt-1 mb-2';
    validationMessage.style.display = 'none';
    
    // Add validation message element below email field
    if (emailInput) {
        // Insert validationMessage after the email input
        emailInput.parentNode.insertBefore(validationMessage, emailInput.nextSibling);
    }
    
    if (signInForm && emailInput && signInButton) {
        // Enable button by default
        signInButton.disabled = false;
        
        let emailTimeout;
        
        // Add event listener for email input to check user existence
        emailInput.addEventListener('input', function() {
            // Clear any existing timeout
            clearTimeout(emailTimeout);
            
            const email = emailInput.value.trim();
            
            // Basic email validation first
            if (!isValidEmail(email)) {
                if (email.length > 0) {
                    validationMessage.textContent = 'Please enter a valid email address';
                    validationMessage.style.display = 'block';
                } else {
                    validationMessage.textContent = '';
                    validationMessage.style.display = 'none';
                }
                return;
            }
            
            // Show loading state
            validationMessage.textContent = 'Checking...';
            validationMessage.style.display = 'block';
            validationMessage.className = 'text-secondary mt-1 mb-2';
            
            // Set a new timeout to prevent excessive API calls
            emailTimeout = setTimeout(function() {
                // Check if the user exists via API call
                checkUserExists(email)
                    .then(exists => {
                        if (exists) {
                            // User exists, enable the button
                            signInButton.disabled = false;
                            validationMessage.textContent = '';
                            validationMessage.style.display = 'none';
                        } else {
                            // User doesn't exist, disable the button
                            signInButton.disabled = true;
                            validationMessage.textContent = 'No account found with this email. Please sign up.';
                            validationMessage.style.display = 'block';
                            validationMessage.className = 'text-danger mt-1 mb-2';
                        }
                    })
                    .catch(error => {
                        console.error('Error checking user:', error);
                        // On error, enable the button as a fallback
                        signInButton.disabled = false;
                        validationMessage.textContent = '';
                        validationMessage.style.display = 'none';
                    });
            }, 500); // Wait for 500ms after the user stops typing
        });
        
        // Add form submit handler
        signInForm.addEventListener('submit', function(event) {
            const email = emailInput.value.trim();
            const password = passwordInput.value;
            
            // Prevent form submission if fields are empty
            if (!email || !password) {
                event.preventDefault();
                
                if (!email) {
                    validationMessage.textContent = 'Please enter your email address';
                    validationMessage.style.display = 'block';
                    validationMessage.className = 'text-danger mt-1 mb-2';
                }
                
                return;
            }
            
            // Prevent form submission if the email is invalid
            if (!isValidEmail(email)) {
                event.preventDefault();
                validationMessage.textContent = 'Please enter a valid email address';
                validationMessage.style.display = 'block';
                validationMessage.className = 'text-danger mt-1 mb-2';
                return;
            }
            
            // Prevent form submission if the button is disabled (user doesn't exist)
            if (signInButton.disabled) {
                event.preventDefault();
                return;
            }
        });
    }
    
    // Validation helper function without using regex
// Validation helper function that only allows emails with exactly one dot in the domain
// Validation helper function that only allows emails with exactly one dot in the domain
function isValidEmail(email) {
    // Check if email is empty or not a string
    if (!email || typeof email !== 'string') {
        return false;
    }
    
    // Trim the email
    email = email.trim();
    
    // Check for @ symbol and position
    const atIndex = email.indexOf('@');
    if (atIndex === -1 || atIndex === 0) {
        return false;
    }
    
    // Get the domain part (after @)
    const domainPart = email.substring(atIndex + 1);
    
    // Check for exactly one dot in the domain part
    const firstDotIndex = domainPart.indexOf('.');
    const lastDotIndex = domainPart.lastIndexOf('.');
    
    // If no dot or more than one dot (first and last dot positions differ)
    if (firstDotIndex === -1 || firstDotIndex !== lastDotIndex) {
        return false;
    }
    
    // Domain cannot end with a dot
    if (firstDotIndex === domainPart.length - 1) {
        return false;
    }
    
    // Check for double @ symbols
    if (email.indexOf('@', atIndex + 1) !== -1) {
        return false;
    }
    
    // Check for invalid characters
    const invalidChars = " <>()\\[\\],;:\"";
    for (let i = 0; i < invalidChars.length; i++) {
        if (email.includes(invalidChars[i])) {
            return false;
        }
    }
    
    // Check parts of the email have some length
    const localPart = email.substring(0, atIndex);
    const topLevelDomain = domainPart.substring(firstDotIndex + 1);
    const secondLevelDomain = domainPart.substring(0, firstDotIndex);
    
    if (localPart.length === 0 || secondLevelDomain.length === 0 || topLevelDomain.length === 0) {
        return false;
    }
    
    return true;
}
    
    // API function to check if user exists
    async function checkUserExists(email) {
        try {
            const response = await fetch(`/api/account/checkuser?email=${encodeURIComponent(email)}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            
            const data = await response.json();
            return data.exists;
        } catch (error) {
            console.error('Error checking user:', error);
            return true; // Default to allowing submission on error
        }
    }
});