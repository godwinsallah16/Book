#!/usr/bin/env node

/**
 * Debug script to validate DATABASE_URL format
 * Run this to check if your DATABASE_URL is valid
 */

const databaseUrl = process.env.DATABASE_URL;

console.log('=== DATABASE_URL Debug Script ===');
console.log('');

if (!databaseUrl) {
    console.log('❌ DATABASE_URL is not set');
    console.log('');
    console.log('To fix this:');
    console.log('1. Go to your Render.com dashboard');
    console.log('2. Navigate to your PostgreSQL database service');
    console.log('3. Click on "Info" tab');
    console.log('4. Copy the "External Database URL"');
    console.log('5. Go to your web service > Environment tab');
    console.log('6. Set DATABASE_URL to the copied value');
    process.exit(1);
}

console.log('✅ DATABASE_URL is set');
console.log('');

// Basic validation
console.log('=== Format Validation ===');
console.log(`Length: ${databaseUrl.length}`);
console.log(`First 50 chars: ${databaseUrl.substring(0, 50)}...`);
console.log('');

// Check for common issues
const issues = [];

if (databaseUrl.startsWith(' ') || databaseUrl.endsWith(' ')) {
    issues.push('❌ Has leading or trailing spaces');
}

if (!databaseUrl.startsWith('postgresql://') && !databaseUrl.startsWith('postgres://')) {
    issues.push('❌ Does not start with postgresql:// or postgres://');
}

if (databaseUrl.startsWith('postgres://')) {
    issues.push('⚠️  Starts with postgres:// (should be postgresql://)');
}

if (databaseUrl.includes(' ')) {
    issues.push('❌ Contains spaces (connection strings should not have spaces)');
}

// Parse the URL
try {
    const url = new URL(databaseUrl);
    console.log('=== Parsed Components ===');
    console.log(`Protocol: ${url.protocol}`);
    console.log(`Username: ${url.username}`);
    console.log(`Password: ${url.password ? '[HIDDEN]' : 'Not set'}`);
    console.log(`Host: ${url.hostname}`);
    console.log(`Port: ${url.port || 'default'}`);
    console.log(`Database: ${url.pathname.substring(1)}`);
    console.log('');
    
    // Validate components
    if (!url.username) {
        issues.push('❌ Username is missing');
    }
    if (!url.password) {
        issues.push('❌ Password is missing');
    }
    if (!url.hostname) {
        issues.push('❌ Hostname is missing');
    }
    if (!url.pathname || url.pathname === '/') {
        issues.push('❌ Database name is missing');
    }
    
} catch (error) {
    issues.push(`❌ Invalid URL format: ${error.message}`);
}

console.log('=== Issues Found ===');
if (issues.length === 0) {
    console.log('✅ No issues found - connection string looks valid!');
} else {
    issues.forEach(issue => console.log(issue));
}

console.log('');
console.log('=== Next Steps ===');
if (issues.length === 0) {
    console.log('Your DATABASE_URL looks good. If you\'re still having issues:');
    console.log('1. Check that your PostgreSQL service is running on Render.com');
    console.log('2. Verify the service is linked to your web service');
    console.log('3. Check the deployment logs for other errors');
} else {
    console.log('Fix the issues above, then:');
    console.log('1. Update DATABASE_URL in your Render.com web service');
    console.log('2. Save the environment variable');
    console.log('3. Wait for automatic redeployment');
}
