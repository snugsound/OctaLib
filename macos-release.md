# Macos release build process

#### Update version numbers:
- OctaLib/OctaLib.csproj
- OctaLib.Desktop/OctaLib.Desktop.app/Contents/Info.plist

#### Publish
dotnet clean OctaLib.Desktop/OctaLib.Desktop.csproj

dotnet publish OctaLib.Desktop/OctaLib.Desktop.csproj -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:UseAppHost=true -o ./publish-arm64
dotnet publish OctaLib.Desktop/OctaLib.Desktop.csproj -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:UseAppHost=true -o ./publish-x64

lipo -create -output ./OctaLib.Desktop/OctaLib.Desktop.app/Contents/MacOS/OctaLib.Desktop \
  ./publish-arm64/OctaLib.Desktop \
  ./publish-x64/OctaLib.Desktop

#### Package dir structure

├── OctaLib.Desktop.app
  └── Contents
      ├── Info.plist
      ├── MacOS
      │   ├── OctaLib.Desktop
      │   ├── OctaLib.Desktop.pdb
      │   ├── OctaLib.pdb
      │   ├── OctaLibCore.pdb
      │   ├── libAvaloniaNative.dylib
      │   ├── libHarfBuzzSharp.dylib
      │   └── libSkiaSharp.dylib
      └── _CodeSignature
          └── CodeResources

#### codesign, package DMG, notorize
codesign --force --verbose --options runtime --entitlements entitlements.plist --sign "<TEAM_NAME>" OctaLib.Desktop.app/Contents/MacOS/OctaLib.Desktop

codesign --force --verbose --deep --options runtime --entitlements entitlements.plist --sign "<TEAM_NAME>" OctaLib.Desktop.app

// validating code signing
codesign -dv --verbose=4 OctaLib.Desktop.app
codesign --verify --deep --strict --verbose=2 OctaLib.Desktop.app

// PACKAGE AS DMG
npx create-dmg OctaLib.Desktop.app ./
mv "Octalib undefined.dmg" OctaLib-new.dmg

// notorise the app
xcrun notarytool submit --apple-id "<APPLE_ID_EMAIL>" --password "<APP_PASSWORD>" --team-id "<TEAM_ID>" --wait OctaLib-v0.1.0.0.dmg

// staple so can run offline
xcrun stapler staple OctaLib-v0.1.0.0.dmg

// validate stapling
xcrun stapler validate OctaLib-v0.1.0.0.dmg

#### Notarising extra commands if there was issue

// if notarising takes ages / drops out with request timeout u can check the progress with
xcrun notarytool history --apple-id "<APPLE_ID_EMAIL>" --password "<APP_PASSWORD>" --team-id "<TEAM_ID>"

// can fetch logg for specific job
xcrun notarytool log <uuid> --apple-id "<APPLE_ID_EMAIL>" --password "<APP_PASSWORD>" --team-id "<TEAM_ID>"

#### extra notes
// way to check all libraries used at runtime
DYLD_PRINT_LIBRARIES=1 ./OctaLib.Desktop.app/Contents/MacOS/OctaLib.Desktop
