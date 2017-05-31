import pandas
import os.path as path
from bs4 import BeautifulSoup
import librosa
import numpy as np
import sys
import getopt
import math
from itertools import filterfalse

SAMPLE_DURATION = .016 #(1.0/30.0)/2.0

def load_mtt_annotations(mtt_path, verbose=False):
    annotations_file = path.join(mtt_path, "annotations_final.csv")
    if verbose:
        print("loading annotations: " + annotations_file)
    annotations = pandas.read_csv(annotations_file, sep='\t', index_col='clip_id')
    return annotations
    
def load_mtt_clip_info(mtt_path, verbose=False):
    clip_info_file = path.join(mtt_path, "clip_info_final.csv")
    if verbose:
        print("loading clip info: " + clip_info_file)
    clip_info = pandas.read_csv(clip_info_file, sep='\t', index_col='clip_id')
    return clip_info
    
def load_mtt_features(mtt_path, clip_info, index, start=0, stop=None, jmp=1, verbose=False):
    if stop == None:
        stop = len(index.values)
    features = [(load_mtt_features_for_clip(mtt_path, clip_info, clip_id, verbose)) for clip_id in index.values[start:stop:jmp]]
    return pandas.concat(features)
    
def load_mtt_audio(mtt_path, clip_info, index, start=0, stop=None, jmp=1, verbose=False):
    audio = [(load_mtt_audio_for_clip(mtt_path, clip_info, clip_id, verbose)) for clip_id in index.values[start:stop:jmp]]
    return pandas.concat(audio)

def load_mtt_features_for_clip(mtt_path, clip_info, clip_id, verbose=False):
    mp3_path = clip_info.ix[clip_id,'mp3_path']
    features_path = path.join(mtt_path,'mp3_echonest_xml',mp3_path+'.xml')
    if verbose:
        print("loading features: " + features_path)
    try:
        soup = BeautifulSoup(open(features_path), "lxml-xml")
    except Exception as e:
        print(e)
        print("couldn't load features: " + features_path)
        return pandas.DataFrame()
        
    track = soup.find('track')
    tempo = float(track["tempo"])
    
    #calculate number of ~16ms frames are in the clip
    duration = float(track["duration"])
    num_frames = int(duration / SAMPLE_DURATION)
    
    #fill beat identification
    beats = track.find_all('beat')
    beat_ts = pandas.DataFrame([0.0], index=[0], columns=["beat"])
    try:
        beat_ts = pandas.DataFrame([1.0 for beat in beats], index=[int(math.ceil(float(beat.tatum.string)/SAMPLE_DURATION)) for beat in beats], columns=["beat"])
    except:
        pass
    beat_ts = beat_ts.reindex(pandas.Index(range(0,num_frames)), fill_value=0.0)
    
    #fill section identification
    sections = track.find_all('section')
    section_ts = pandas.DataFrame([0.0], index=[0], columns=["section"])
    try:
        section_ts = pandas.DataFrame([1.0 for section in sections], index=[int(math.ceil(float(section["start"])/SAMPLE_DURATION)) for section in sections], columns=["section"])
    except:
        pass
        
    section_ts = section_ts.reindex(pandas.Index(range(0,num_frames)), fill_value=0.0)
    
    tempo_ts = pandas.DataFrame([tempo for i in range(0, num_frames)], pandas.Index(range(0,num_frames)), columns=["tempo"])
    
    merged = pandas.concat([beat_ts, section_ts, tempo_ts], axis=1, join="outer")
    merged = merged.fillna(0.0)
    
    merged['clip_id'] = clip_id
    merged.set_index('clip_id', append=True, inplace=True)
    merged = merged.reorder_levels(['clip_id', 0])
    
    if verbose:
        print("tempo: " + str(tempo))
        print(merged.shape)
        print(merged.head)

    return merged
    
def load_mtt_audio_for_clip(mtt_path, clip_info, clip_id, verbose=False):
    mp3_path = clip_info.ix[clip_id,'mp3_path']
    mp3_file_path = path.join(mtt_path, 'mp3.zip',mp3_path)
    if verbose:
        print("loading audio: " + mp3_file_path)
    samples, sample_rate = librosa.load(mp3_file_path, sr=16000, mono=True)
    
    frames = [samples[256*i:256*i+256] for i in range(0, int(len(samples)/256))]
    num_frames = len(frames)
    
    if verbose:
        print("number of frames: " + str(num_frames))
    
    multi_index = [np.array([clip_id for i in range(0, num_frames)]),
                   np.array(range(0,num_frames))]
    sample_df = pandas.DataFrame(frames, index=multi_index)
    
    if verbose:
        print(sample_df.shape)
        print(sample_df.head())

    return sample_df

class FilterDelegate:
    clip_info = None
    mtt_path = None
    
    def __call__(self, clip_id):
        return self.prune_mtt_filter(clip_id)
    
    def prune_mtt_filter(self, clip_id):
        mp3_path = self.clip_info.ix[clip_id,'mp3_path']
        mp3_file_path = path.join(self.mtt_path, 'mp3.zip',mp3_path)
        features_path = path.join(self.mtt_path,'mp3_echonest_xml',mp3_path+'.xml')
        both_exist = path.isfile(mp3_file_path) and path.isfile(features_path)
            
        return not both_exist

def prune_mtt_items_without_audio_or_feautures(index, mtt_path, clip_info):
    delegate = FilterDelegate()
    delegate.clip_info = clip_info
    delegate.mtt_path = mtt_path
    pruned = [i for i in filterfalse(delegate, index.values)]
    return pandas.Index(pruned).intersection(index)

def load_mtt(mtt_path, load_features=False, load_audio=False, start=0, stop=None, jmp=1, verbose=False):
    annotations = load_mtt_annotations(mtt_path, verbose)
    clip_info = load_mtt_clip_info(mtt_path, verbose)
    
    clip_index = clip_info.index
    annotations_index = annotations.index
    
    # shrink the datasets to the intersection of the data with the right supplemental files
    intersected_index = clip_index.intersection(annotations_index)
    intersected_index = prune_mtt_items_without_audio_or_feautures(intersected_index, mtt_path, clip_info)
    clip_info = clip_info.reindex(intersected_index)
    annotations = annotations.reindex(intersected_index)
    
    # use the subset specificed by the user
    clip_info = clip_info.ix[intersected_index.values[start:stop:jmp]]
    annotations = annotations.ix[intersected_index.values[start:stop:jmp]]
    
    features = pandas.DataFrame()
    if load_features:
        features = load_mtt_features(mtt_path, clip_info, intersected_index, start, stop, jmp, verbose)
    audio = pandas.DataFrame()
    if load_audio:
        audio = load_mtt_audio(mtt_path, clip_info, intersected_index, start, stop, jmp, verbose)
    
    
    if features.shape[0] != audio.shape[0]:
        print("start: " + str(start) + " stop: " + str(stop) + " jmp: " + str(jmp))
    
    #if verbose:
    print("annotations: " + str(annotations.shape))
    print("clip_info: " + str(clip_info.shape))
    print("features: " + str(features.shape))
    print("audio: " + str(audio.shape))
    return (annotations, clip_info, features, audio)

def main(args):
    try:
        opts, args = getopt.getopt(sys.argv[1:], "p:v", [ "path=", "start=", "stop="])
    except getopt.GetoptError as err:
        # print help information and exit:
        print(str(err))  # will print something like "option -a not recognized"
        usage()
        sys.exit(2)
    mtt_path = None
    verbose = False
    start = 0
    stop = 5
    for o, a in opts:
        if o in ("-p", "--path"):
            mtt_path = a
        elif o in ("--stop"):
            stop=int(a)
        elif o in ("--start"):
            start=int(a)
        elif o in ("-v"):
            verbose = True
        else:
            assert False, "unhandled option"
    print("loading MTT from: " + mtt_path)
    annotations, clip_info, features, audio = load_mtt(mtt_path, True, True, start, stop, 1, verbose=verbose)
    
    #feature = load_mtt_features_for_clip(mtt_path, clip_info, clip_info.index.values[0], verbose)
    #audio = load_mtt_audio_for_clip(mtt_path, clip_info, clip_info.index.values[0], verbose)
 
if __name__ == "__main__":
    main(sys.argv)