﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarfieldUtils.MathUtils
{
    /**
     * <summary>    A point class used for K Means Clustering. </summary>
     */
     // TODO: convert to Vec2D
    public class KMeansPoint
    {
        public double x;
        public double y;

        public double distance(KMeansPoint p1)
        {
            return Math.Sqrt(Math.Pow(p1.x - x, 2) + Math.Pow(p1.y - y, 2));
        }
    }

    /**
     * <summary>    Encapsulates the result of a K Means clustering run. </summary>
     */

    public class KMeansResult
    {
        /** <summary>    The centroids of the clusters. </summary> */
        public KMeansPoint[] centroids;
        /** <summary>    Which points are in what centroids. </summary> */
        public int[] membership;
    }

    /**
     * <summary>    Class for providing K Means related computations. </summary>
     *
     * <remarks>    Volar, 2/13/2017. </remarks>
     */

    public class KMeans
    {
        /**
         * <summary>    Calculates centroids of numClusters clusters using K Means. </summary>
         *
         * <param name="points">        The points. </param>
         * <param name="numClusters">   Number of clusters. </param>
         * <param name="numIterations"> Number of iterations. </param>
         *
         * <returns>    The found k means. </returns>
         */

        public static KMeansResult FindKMeans(KMeansPoint[] points, int numClusters, int numIterations)
        {
            // initial clustering
            Random rand = new Random();
            int[] clusterMap = new int[points.Length];
            int[] clusterCount = new int[numClusters];
            KMeansPoint[] means = new KMeansPoint[numClusters];

            for(int i = 0; i < points.Length; i++)
            {
                clusterMap[i] = i % numClusters;
            }

            bool changed;
            for(int i = 0; i < numIterations; i++)
            {
                changed = false;
                // reset means
                for (int j = 0; j < numClusters; j++)
                {
                    means[j] = new KMeansPoint();
                    means[j].x = 0;
                    means[j].y = 0;
                }

                for (int j = 0; j < numClusters; j++)
                {
                    clusterCount[j] = 0;
                }

                for (int j = 0; j < points.Length; j++)
                {
                    clusterCount[clusterMap[j]]++;
                }

                // find new means
                for (int j = 0; j < points.Length; j++)
                {
                    means[clusterMap[j]].x += points[j].x;
                    means[clusterMap[j]].y += points[j].y;
                }

                for (int j = 0; j < numClusters; j++)
                {
                    means[j].x /= clusterCount[j];
                    means[j].y /= clusterCount[j];
                }

                // update clusters
                // optimization opportunity: switch to space partitioning or some other nearest neighbor alg https://en.wikipedia.org/wiki/Nearest_neighbor_search
                for (int j = 0; j < points.Length; j++)
                {
                    int k = 0;
                    int minIndex = 0;
                    double min = double.PositiveInfinity;
                    do
                    {
                        double dist = means[k].distance(points[j]);
                        if(dist < min)
                        {
                            minIndex = k;
                            min = dist;
                        }
                        k++;
                    } while (k < numClusters);

                    if(clusterMap[j] != minIndex)
                    {
                        changed = true;
                    }

                    clusterMap[j] = minIndex;
                }

                // if we haven't changed, go ahead and quit
                if (!changed)
                {
                    break;
                }
            }
            KMeansResult result = new KMeansResult();
            result.centroids = new KMeansPoint[means.Length];
            means.CopyTo(result.centroids, 0);
            result.membership = new int[clusterMap.Length];
            clusterMap.CopyTo(result.membership, 0);
            return result;
        }

        /**
         * <summary>    clusters a set with an unknown number of clusters runs kmeans for k in the range
         *              1 to maxClusters finds the k where increasing k gives diminishing returns
         *              https://en.wikipedia.org/wiki/Determining_the_number_of_clusters_in_a_data_set#An_Information_Theoretic_Approach. </summary>
         *
         * <param name="points">        The points. </param>
         * <param name="maxClusters">   The maximum clusters. </param>
         * <param name="numIterations"> Number of iterations. </param>
         *
         * <returns>    A KMeansResult. </returns>
         */

        public static KMeansResult Cluster(KMeansPoint[] points, int maxClusters, int numIterations)
        {
            KMeansResult[] results = new KMeansResult[maxClusters];
            double[] distortions = new double[maxClusters];
            for(int i = 1; i < maxClusters; i++)
            {
                results[i] = FindKMeans(points, i, numIterations);
                distortions[i] = getDistortion(results[i], points);
            }

            int index = 0;
            double distortion = distortions[0];
            for(int i = 1; i < maxClusters; i++)
            {
                if(distortions[i] - distortions[i-1] > distortion)
                {
                    index = i;
                    distortion = distortions[i] - distortions[i - 1];
                }
            }

            return results[index];
        }

        /**
         * <summary>    there are more complicated/better ways of doing this for now, we'll just use the
         *              sum of the distances between the points their centroids. </summary>
         *
         * <param name="result">    The result. </param>
         * <param name="points">    The points. </param>
         *
         * <returns>    The distortion. </returns>
         */

        private static double getDistortion(KMeansResult result, KMeansPoint[] points)
        {
            // TODO: http://stats.stackexchange.com/questions/62092/bottom-to-top-explanation-of-the-mahalanobis-distance
            double distortion = 0;
            for (int i = 0; i < points.Length; i++)
            {
                distortion += points[i].distance(result.centroids[result.membership[i]]);
            }
            return 1/distortion;
        }
    }
}
