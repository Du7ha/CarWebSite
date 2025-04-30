// Add this to your site.js file or include directly in the layout

document.addEventListener('DOMContentLoaded', function() {
    // Get references to the sign-in form elements
    const signInForm = document.querySelector('form[action*="SignIn"]');
    const emailInput = document.getElementById('email');
    const signInButton = signInForm?.querySelector('button[type="submit"]');
    const validationMessage = document.createElement('div');
    
    // Add validation message element below email field
    if (emailInput) {
        validationMessage.className = 'text-danger mt-1 mb-2';
        validationMessage.style.display = 'none';
        emailInput.parentNode.appendChild(validationMessage);
    }
    
    if (signInForm && emailInput && signInButton) {
        let emailTimeout;
        
        // Add event listener for email input to check user existence
        emailInput.addEventListener('input', function() {
            // Clear any existing timeout
            clearTimeout(emailTimeout);
            
            // Set a new timeout to prevent excessive API calls
            emailTimeout = setTimeout(function() {
                const email = emailInput.value.trim();
                
                // Basic email validation
                if (!isValidEmail(email)) {
                    validationMessage.textContent = '';
                    validationMessage.style.display = 'none';
                    return;
                }
                
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
                        }
                    })
                    .catch(error => {
                        console.error('Error checking user:', error);
                    });
            }, 500); // Wait for 500ms after the user stops typing
        });
        
        // Add form submit handler
        signInForm.addEventListener('submit', function(event) {
            const email = emailInput.value.trim();
            
            // Prevent form submission if the email is invalid or user doesn't exist
            if (signInButton.disabled || !isValidEmail(email)) {
                event.preventDefault();
            }
        });
    }
    
    // Validation helper function
    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
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