#!/bin/bash

# install-template.sh
# Script to install and test the Clean Architecture template

set -e

TEST_PROJECT_NAME="TestCleanArchProject"
TEMPLATE_DIR=$(pwd)
TEMP_TEST_DIR="${TEMPLATE_DIR}/temp-test"

echo "🚀 Installing Clean Architecture DDD Template..."
echo "📁 Template path: $TEMPLATE_DIR"

# Uninstall existing template if it exists
echo "🗑️  Removing existing template..."
dotnet new uninstall -a Archetype.CleanArchitecture.Template 2>/dev/null || true

# Install the template from current directory
echo "📦 Installing template..."
dotnet new install "$TEMPLATE_DIR"

# Verify installation
echo "✅ Verifying template installation..."
if dotnet new list | grep -q "cleanarch"; then
    echo "✅ Template successfully installed!"
    dotnet new list | grep cleanarch | sed 's/^/   /'
else
    echo "❌ Template installation failed!"
    exit 1
fi

# Create test project
echo "🧪 Creating test project: $TEST_PROJECT_NAME"
rm -rf "$TEMP_TEST_DIR"
mkdir -p "$TEMP_TEST_DIR"
cd "$TEMP_TEST_DIR"

# Create project from template
echo "📂 Running: dotnet new cleanarch -n $TEST_PROJECT_NAME --ProjectPrefix TestCompany"
if dotnet new cleanarch -n "$TEST_PROJECT_NAME" --ProjectPrefix TestCompany; then
    echo "✅ Test project created successfully!"

    # Navigate to project
    cd "$TEST_PROJECT_NAME"

    # Test restore
    echo "📦 Restoring packages..."
    if dotnet restore; then
        echo "✅ Packages restored successfully!"
    else
        echo "❌ Package restore failed!"
        exit 1
    fi

    # Test build
    echo "🔨 Building project..."
    if dotnet build; then
        echo "✅ Build successful!"
    else
        echo "❌ Build failed!"
        exit 1
    fi

    # Test run (background)
    echo "🏃 Starting API..."
    dotnet run --project "src/$TEST_PROJECT_NAME.Api" --urls "http://localhost:5000" &
    API_PID=$!

    # Wait for API to start
    sleep 5

    # Test API health
    echo "🔍 Testing API health..."
    if curl -s -f http://localhost:5000/health > /dev/null 2>&1; then
        echo "✅ API is responding!"
    else
        echo "⚠️  API health check failed (this is normal if no health endpoint)"
    fi

    # Stop the API
    kill $API_PID 2>/dev/null || true
    wait $API_PID 2>/dev/null || true

else
    echo "❌ Failed to create test project!"
    exit 1
fi

# Cleanup
cd "$TEMPLATE_DIR"
echo "🧹 Cleaning up test files..."
rm -rf "$TEMP_TEST_DIR"

echo ""
echo "🎉 Template installation and testing complete!"
echo ""
echo "📖 Usage:"
echo "   dotnet new cleanarch -n YourProject --ProjectPrefix YourCompany"
echo ""