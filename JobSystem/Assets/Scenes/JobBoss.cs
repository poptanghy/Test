using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class JobBoss : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartMyJob();
        StartJobParallelFor();
        Debug.Log("AfterJob");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartMyJob()
    {
        // Create a native array of a single float to store the result. This example waits for the job to complete for illustration purposes
        NativeArray<float> result = new NativeArray<float>(1, Allocator.TempJob);

        // Set up the job data
        MyJob jobData = new MyJob();
        jobData.a = 10;
        jobData.b = 10;
        jobData.result = result;

        JobB jobB = new JobB();
        jobB.result = result;

        // Schedule the job
        JobHandle handle = jobData.Schedule();
        JobHandle hB = jobB.Schedule(handle);

        // Wait for the job to complete
        hB.Complete();

        // All copies of the NativeArray point to the same memory, you can access the result in "your" copy of the NativeArray
        float aPlusB = result[0];
        Debug.Log(aPlusB);

        // Free the memory allocated by the result array
        result.Dispose();
    }

    void StartJobParallelFor()
    {
        NativeArray<float> a = new NativeArray<float>(2, Allocator.TempJob);
        NativeArray<float> b = new NativeArray<float>(2, Allocator.TempJob);
        NativeArray<float> result = new NativeArray<float>(2, Allocator.TempJob);

        a[0] = 1.1f;
        b[0] = 2.2f;
        a[1] = 3.3f;
        b[1] = 4.4f;

        MyParallelJob jobData = new MyParallelJob();
        jobData.a = a;
        jobData.b = b;
        jobData.result = result;

        // Schedule the job with one Execute per index in the results array and only 1 item per processing batch
        JobHandle handle = jobData.Schedule(result.Length, 1);

        // Wait for the job to complete
        handle.Complete();
        Debug.Log(result[0] + " " + result[1]);

        // Free the memory allocated by the arrays
        a.Dispose();
        b.Dispose();
        result.Dispose();
    }

    void Combine()
    {
        NativeArray<JobHandle> handles = new NativeArray<JobHandle>(new JobHandle[] { new JobHandle(), new JobHandle() }, Allocator.TempJob);
        JobHandle jh = JobHandle.CombineDependencies(handles);
    }
}
