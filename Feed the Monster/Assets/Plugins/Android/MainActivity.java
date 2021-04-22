package au.org.libraryforall.feedthemonster;

import com.unity3d.player.UnityPlayerActivity;
import android.os.Bundle;
import android.os.Build;
import android.util.Log;
import au.org.libraryforall.logger;
import org.json.JSONException;
import org.json.JSONObject;

public class MainActivity extends UnityPlayerActivity {
    private static final String TAG = "FeedTheMonster";
    private Logger logger;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        Log.d(TAG, "Created!");
        logger = new Logger("FeedTheMonster", getApplicationContext());
    }
    
    public void tagScreen(String event) {
        try {
            Log.d(TAG, event);
            JSONObject eventValue = new JSONObject();
            eventValue.put("category", event);
            eventValue.put("action", "tagScreen");
            logger.logEvent(eventValue.toString());
        } catch (JSONException ex) {
            ex.printStackTrace();
            Log.e(TAG,ex.getMessage());
        }
    }
    
    public void logEvent(String category, String action, String label, String value) {
        try {
            String event = "category " + category + ", action " + action + ", label " + label + ", value " + value;
            Log.d(TAG, event);
            JSONObject eventValue = new JSONObject();
            eventValue.put("category", category);
            eventValue.put("action", action);
            eventValue.put("label", label);
            eventValue.put("value", value);
            logger.logEvent(eventValue.toString());
        } catch (JSONException ex) {
            ex.printStackTrace();
            Log.e(TAG,ex.getMessage());
        }
    }
}