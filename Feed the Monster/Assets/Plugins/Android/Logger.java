package au.org.libraryforall.logger;

import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Environment;
import android.provider.Settings;
import android.util.Log;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Calendar;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

import java.net.NetworkInterface;
import java.util.Collections;
import java.util.List;

/**
 * Created by ingtellect on 7/14/17.
 */

public class Logger {
    private static Logger logger;
    private static final String TAG = "Logger";
    private File basePath;
    private String appName;
    private String shortAppName;
    private Context _ctnx;

    private String deviceID;

    public static String getMacAddr() {
        try {
            List<NetworkInterface> all = Collections.list(NetworkInterface.getNetworkInterfaces());
            for (NetworkInterface nif : all) {
                if (!nif.getName().equalsIgnoreCase("wlan0")) continue;

                byte[] macBytes = nif.getHardwareAddress();
                if (macBytes == null) {
                    return "";
                }

                StringBuilder res1 = new StringBuilder();
                for (byte b : macBytes) {
                    // res1.append(Integer.toHexString(b & 0xFF) + ":");
                    res1.append(String.format("%02X:", b));
                }

                if (res1.length() > 0) {
                    res1.deleteCharAt(res1.length() - 1);
                }
                return res1.toString();
            }
        } catch (Exception ex) {
            //handle exception
        }
        return "";
    }

    public Logger(String appName, Context ctnx) {
        this.appName = appName;
        int pos;
        if ((pos = appName.lastIndexOf('.')) != -1) {
            this.shortAppName = appName.substring(pos + 1);
        } else
            this.shortAppName = appName;

        //basePath = new File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOCUMENTS).toString() + "/" + installId);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            basePath = new File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOCUMENTS).toString() + "/" + "logs");
        } else {
            basePath = new File(Environment.getExternalStorageDirectory() + "/Documents" + "/" + "logs");
        }

        if (!basePath.exists()) {
            basePath.mkdirs();
        }
        _ctnx = ctnx;
        deviceID = Settings.Secure.getString(ctnx.getContentResolver(), Settings.Secure.ANDROID_ID);
    }
    
    public static Logger getInstance(String appName, Context ctnx) {
        if (logger == null) {
            logger = new Logger(appName, ctnx);
        }
        return logger;
    }

    public void tagScreen(String screenName) {
        try {
            JSONObject eventValue = new JSONObject();
            eventValue.put("category", screenName);
            eventValue.put("action", "tagScreen");
            logEvent(eventValue.toString());
        } catch (JSONException ex) {
            ex.printStackTrace();
            Log.e(TAG, ex.getMessage());
        }
    }

    public void logEvent(String category, String event, String label, double value) {
        try {
            JSONObject eventValue = new JSONObject();
            eventValue.put("category", category);
            eventValue.put("event", event);
            eventValue.put("label", label);
            eventValue.put("value", value);
            logEvent(eventValue.toString());
        } catch (JSONException ex) {
            ex.printStackTrace();
            Log.e(TAG, ex.getMessage());
        }
    }

    public String getZipName() {
        StringBuilder name = new StringBuilder();
        name.append(deviceID).append(".").append(System.currentTimeMillis() / 1000);
        return name.toString();
    }

    public String normAppName() {
        return appName.replace('.', '_');
    }

    public String lastLogPathName() {
        return normAppName() + "." + "lastlog" + ".txt";
    }

    private byte[] readAllBytes(File f) {
        byte[] b = new byte[(int) f.length()];
        try {
            FileInputStream fis = new FileInputStream(f);
            fis.read(b);
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }
        return b;
    }

    public void logEvent(String eventString) {

        File logPath = new File(basePath + "/" + lastLogPathName());
        if (!logPath.exists()) {
            try {
                logPath.createNewFile();
            } catch (Exception e) {
            }
        }
        try {
            FileWriter fw = new FileWriter(logPath.getAbsoluteFile(), true);
            BufferedWriter bw = new BufferedWriter(fw);

            JSONObject eventJson = new JSONObject(eventString);
            JSONObject logJson = new JSONObject();
            logJson.put("timeStamp", (double) (System.currentTimeMillis() / 1000));
            logJson.put("device_id", deviceID);
            Iterator<String> keys = eventJson.keys();
            while (keys.hasNext()) {
                String key = keys.next();
                logJson.put(key, eventJson.get(key));
            }
            bw.write(logJson.toString() + "\n");
            bw.close();
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }
        long logSize = logPath.length();
        if (logSize < 100 * 1024) {
            return;
        }
        String header = normAppName() + ".";
        String zipFooter = ".log.zip";
        String txtFooter = ".log.txt";
        try {
            File zipPath = new File(basePath + "/" + (header + getZipName() + zipFooter));
            ZipOutputStream zipOS = new ZipOutputStream(new FileOutputStream(zipPath));
            {
                ZipEntry e = new ZipEntry(header + getZipName() + txtFooter);
                zipOS.putNextEntry(e);
                byte[] bytes = readAllBytes(logPath);
                zipOS.write(bytes, 0, bytes.length);
                zipOS.closeEntry();
            }
            zipOS.close();
        } catch (Exception e) {
            Log.e(TAG, e.getMessage());
        }
        logPath.delete();
    }

    public String extractLogToFile() {
        Log.d(TAG, "logger caught unhandled exception");
        PackageManager manager = _ctnx.getPackageManager();
        PackageInfo info = null;
        try {
            info = manager.getPackageInfo(_ctnx.getPackageName(), 0);
        } catch (PackageManager.NameNotFoundException e2) {
            Log.e(TAG, e2.getMessage());
        }
        String model = Build.MODEL;
        if (!model.startsWith(Build.MANUFACTURER))
            model = Build.MANUFACTURER + " " + model;
        String documentsPath = Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOCUMENTS).toString();
        String fullName = documentsPath + "/crashlog." + normAppName() + ".txt";
        File file = new File(fullName);
        if (!file.exists()) {
            try {
                file.createNewFile();
            } catch (IOException e) {
                e.printStackTrace();
                Log.e(TAG, e.getMessage());
            }
        }
        InputStreamReader reader = null;
        FileWriter writer = null;
        try {
            // For Android 4.0 and earlier, you will get all app's log output, so filter it to
            // mostly limit it to your app's output.  In later versions, the filtering isn't needed.
            String cmd = (Build.VERSION.SDK_INT <= Build.VERSION_CODES.ICE_CREAM_SANDWICH_MR1) ?
                    "logcat -d -v time " + appName + ":v dalvikvm:v System.err:v *:s" :
                    "logcat -d -v time";

            // get input stream
            Process process = Runtime.getRuntime().exec(cmd);
            reader = new InputStreamReader(process.getInputStream());

            // write output stream
            writer = new FileWriter(file);
            writer.write("Android version: " + Build.VERSION.SDK_INT + "\n");
            writer.write("Device: " + model + "\n");
            writer.write("App name: " + appName);
            writer.write("App version: " + (info == null ? "(null)" : info.versionCode) + "\n");

            char[] buffer = new char[10000];
            do {
                int n = reader.read(buffer, 0, buffer.length);
                if (n == -1)
                    break;
                writer.write(buffer, 0, n);
            } while (true);

            reader.close();
            writer.close();
        } catch (IOException e) {
            if (writer != null)
                try {
                    writer.close();
                } catch (IOException e1) {
                    Log.e(TAG, e1.getMessage());
                }
            if (reader != null)
                try {
                    reader.close();
                } catch (IOException e1) {
                    Log.e(TAG, e1.getMessage());
                }

            // You might want to write a failure message to the log here.
            Log.e(TAG, "logger failed to write crash log + \n" + e.getMessage());
            return null;
        }

        return fullName;
    }
}
