using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KmeansClustering : MonoBehaviour
{
    public double[][] allMeans;
    public int[] StartClustering(List<double> f1, List<double> f2, List<double> f3, int numClusters)
    {
        Debug.Log("Setting numClusters to " + numClusters);

        int n = f1.Count;
        double[][] rawData = new double[n][];
        List<double> vals = new List<double>();

        for(int i = 0; i < n; ++i) {
            rawData[i] = new double[] {f1[i], f2[i]};
        }

        int[] clustering = Cluster(rawData, numClusters);
        /*for(int i = 0; i < clustering.Length; ++i)
            Debug.Log(clustering[i]);*/

        //gameObject.GetComponent<GraphScript>().ShowGraph(rawData, clustering);

        double[] shortestDists = new double[numClusters];
        //double[][] nearestPts = new double[clustering.Length][];
        int[] nearestPts = new int[numClusters];
        for(int i = 0; i < shortestDists.Length; ++i) {
            shortestDists[i] = Int32.MaxValue;
        }
        for(int i = 0; i < clustering.Length; ++i) {
            switch(clustering[i]) {
                case 0: double newDist = Func.DistTo(new Vector2((float)rawData[i][0], (float)rawData[i][1]), new Vector2((float)allMeans[0][0], (float)allMeans[0][1]));
                        
                        if(newDist <= shortestDists[0]) {
                            shortestDists[0] = newDist;
                            //nearestPts[0] = rawData[i];
                            nearestPts[0] = i;
                            Debug.Log(new Vector2((float)allMeans[0][0], (float)allMeans[0][1]) + " : " + newDist + " at index " + i);
                        }
                        break;
                case 1: newDist = Func.DistTo(new Vector2((float)rawData[i][0], (float)rawData[i][1]), new Vector2((float)allMeans[1][0], (float)allMeans[1][1]));
                        
                        if(newDist <= shortestDists[1]) {
                            shortestDists[1] = newDist;
                            //nearestPts[0] = rawData[i];
                            nearestPts[1] = i;
                            Debug.Log(new Vector2((float)allMeans[1][0], (float)allMeans[1][1]) + " : " + newDist + " at index " + i);
                        }
                        break;
                case 2: newDist = Func.DistTo(new Vector2((float)rawData[i][0], (float)rawData[i][1]), new Vector2((float)allMeans[2][0], (float)allMeans[2][1]));
                        if(newDist <= shortestDists[2]) {
                            shortestDists[2] = newDist;
                            //nearestPts[0] = rawData[i];
                            nearestPts[2] = i;
                            Debug.Log(new Vector2((float)allMeans[2][0], (float)allMeans[2][1]) + " : " + newDist + " at index " + i);
                        }
                        break;
                case 3: newDist = Func.DistTo(new Vector2((float)rawData[i][0], (float)rawData[i][1]), new Vector2((float)allMeans[3][0], (float)allMeans[3][1]));
                        if(newDist <= shortestDists[3]) {
                            shortestDists[3] = newDist;
                            //nearestPts[0] = rawData[i];
                            nearestPts[3] = i;
                            Debug.Log(new Vector2((float)allMeans[3][0], (float)allMeans[3][1]) + " : " + newDist + " at index " + i);
                        }
                        break;
                case 4: newDist = Func.DistTo(new Vector2((float)rawData[i][0], (float)rawData[i][1]), new Vector2((float)allMeans[4][0], (float)allMeans[4][1]));
                        if(newDist <= shortestDists[4]) {
                            shortestDists[4] = newDist;
                            //nearestPts[0] = rawData[i];
                            nearestPts[4] = i;
                            Debug.Log(new Vector2((float)allMeans[4][0], (float)allMeans[4][1]) + " : " + newDist + " at index " + i);
                        }
                        break;
                case 5: newDist = Func.DistTo(new Vector2((float)rawData[i][0], (float)rawData[i][1]), new Vector2((float)allMeans[5][0], (float)allMeans[5][1]));
                        if(newDist <= shortestDists[5]) {
                            shortestDists[5] = newDist;
                            //nearestPts[0] = rawData[i];
                            nearestPts[5] = i;
                            Debug.Log(new Vector2((float)allMeans[5][0], (float)allMeans[5][1]) + " : " + newDist + " at index " + i);
                        }
                        break;
            }
        }

        Debug.Log("Kmeans Complete!");
        return nearestPts;
    }

    public int[] Cluster(double[][] rawData, int numClusters)
    {
        double[][] data = Normalized(rawData);
        bool changed = true; bool success = true;
        int[] clustering = InitClustering(data.Length, numClusters, 0);
        double[][] means = Allocate(numClusters, data[0].Length);
        int maxCount = data.Length * 10;
        int ct = 0;
        while (changed == true && success == true && ct < maxCount) {
            ++ct;
            success = UpdateMeans(data, clustering, means);
            changed = UpdateClustering(data, clustering, means);
        }

        allMeans = means;

        return clustering;
    }

    private static double[][] Normalized(double[][] rawData)
    {
        double[][] result = new double[rawData.Length][];
        for (int i = 0; i < rawData.Length; ++i)
        {
            result[i] = new double[rawData[i].Length];
            Array.Copy(rawData[i], result[i], rawData[i].Length);
        }

        for (int j = 0; j < result[0].Length; ++j)
        {
            double colSum = 0.0;
            for (int i = 0; i < result.Length; ++i)
            colSum += result[i][j];
            double mean = colSum / result.Length;
            double sum = 0.0;
            for (int i = 0; i < result.Length; ++i)
            sum += (result[i][j] - mean) * (result[i][j] - mean);
            double sd = sum / result.Length;
            for (int i = 0; i < result.Length; ++i)
            result[i][j] = (result[i][j] - mean) / sd;
        }
        return result;
    }

    private static int[] InitClustering(int numTuples, int numClusters, int seed)
    {
        System.Random random = new System.Random(seed);
        int[] clustering = new int[numTuples];
        for (int i = 0; i < numClusters; ++i)
            clustering[i] = i;
        for (int i = numClusters; i < clustering.Length; ++i)
            clustering[i] = random.Next(0, numClusters);
        return clustering;
    }

    private static bool UpdateMeans(double[][] data, int[] clustering, double[][] means)
    {
        int numClusters = means.Length;
        int[] clusterCounts = new int[numClusters];
        for (int i = 0; i < data.Length; ++i)
        {
            int cluster = clustering[i];
            ++clusterCounts[cluster];
        }

        for (int k = 0; k < numClusters; ++k)
            if (clusterCounts[k] == 0)
            return false;

        for (int k = 0; k < means.Length; ++k)
            for (int j = 0; j < means[k].Length; ++j)
            means[k][j] = 0.0;

        for (int i = 0; i < data.Length; ++i)
        {
            int cluster = clustering[i];
            for (int j = 0; j < data[i].Length; ++j)
            means[cluster][j] += data[i][j]; // accumulate sum
        }

        for (int k = 0; k < means.Length; ++k)
            for (int j = 0; j < means[k].Length; ++j)
            means[k][j] /= clusterCounts[k]; // danger of div by 0
        return true;
    }

    private static double[][] Allocate(int numClusters, int numColumns)
    {
        double[][] result = new double[numClusters][];
        for (int k = 0; k < numClusters; ++k)
            result[k] = new double[numColumns];
        return result;
    }

    private static bool UpdateClustering(double[][] data, int[] clustering, double[][] means)
    {
        int numClusters = means.Length;
        bool changed = false;

        int[] newClustering = new int[clustering.Length];
        Array.Copy(clustering, newClustering, clustering.Length);

        double[] distances = new double[numClusters];

        for (int i = 0; i < data.Length; ++i) 
        {
            for (int k = 0; k < numClusters; ++k)
            distances[k] = Distance(data[i], means[k]);

            int newClusterID = MinIndex(distances);
            if (newClusterID != newClustering[i])
            {
            changed = true;
            newClustering[i] = newClusterID;
            }
        }

        if (changed == false)
            return false;

        int[] clusterCounts = new int[numClusters];
        for (int i = 0; i < data.Length; ++i)
        {
            int cluster = newClustering[i];
            ++clusterCounts[cluster];
        }

        for (int k = 0; k < numClusters; ++k)
            if (clusterCounts[k] == 0)
            return false;

        Array.Copy(newClustering, clustering, newClustering.Length);
        return true; // no zero-counts and at least one change
    }

    private static double Distance(double[] tuple, double[] mean)
    {
        double sumSquaredDiffs = 0.0;
        for (int j = 0; j < tuple.Length; ++j)
            sumSquaredDiffs += Math.Pow((tuple[j] - mean[j]), 2);
        return Math.Sqrt(sumSquaredDiffs);
    }

    private static int MinIndex(double[] distances)
    {
        int indexOfMin = 0;
        double smallDist = distances[0];
        for (int k = 0; k < distances.Length; ++k) {
            if (distances[k] < smallDist) {
            smallDist = distances[k];
            indexOfMin = k;
            }
        }
        return indexOfMin;
    }
}
