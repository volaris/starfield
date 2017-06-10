from keras.models import Sequential
from keras.layers import LSTM, Dense, Conv1D
from keras.layers.core import Dropout
from keras import backend
import numpy as np
from load_mtt import load_mtt
import sys, getopt
from ast import literal_eval as make_tuple
from keras.optimizers import SGD, Adam, RMSprop, Adadelta
import matplotlib.pyplot as plt

def buildModel(batch_size, output_size):
    data_dim = 256
    timesteps = 1
    num_classes = 3
    model = Sequential()
    model.add(Conv1D(filters=32, 
                     kernel_size=1, 
                     input_shape=(None, 256),
                     activation='relu',
                     batch_input_shape=(batch_size, timesteps, data_dim)))
    model.add(Dense(32, activation='relu'))
    model.add(Dropout(.25))
    model.add(Dense(32, activation='relu'))
    model.add(Dropout(.25))
    model.add(LSTM(32, return_sequences=True, stateful=True))
    model.add(LSTM(32, return_sequences=True, stateful=True))
    model.add(LSTM(32, stateful=True))
    model.add(Dense(output_size, activation='relu'))

    model.compile(loss='mse',
                  optimizer=Adam(),
                  metrics=[])
    
    return model

def buildCombinedModel(batch_size):
    return buildModel(batch_size, 3)

def buildBPMModel(batch_size):
    return buildModel(batch_size, 1)
    
def buildBeatsModel(batch_size):
    return buildModel(batch_size, 2)

def main(args):
    try:
        opts, args = getopt.getopt(sys.argv[1:], "p:v", [ "path=", 
                                                          "data_size=", 
                                                          "rounds=", 
                                                          "save=", 
                                                          "load=", 
                                                          "evaluate=",
                                                          "train=" ])
    except getopt.GetoptError as err:
        # print help information and exit:
        print(str(err))  # will print something like "option -a not recognized"
        usage()
        sys.exit(2)
    mtt_path = None
    verbose = False
    data_size = 5 # small default for quick testing
    rounds = 1
    save = False
    save_path = None
    load = False
    load_path = None
    eval = False
    eval_sample = None
    batch_size = 100
    train_type = None
    for o, a in opts:
        if o in ("-p", "--path"):
            mtt_path = a
        elif o in ("-v"):
            verbose = True
        elif o in ("--data_size"):
            data_size = int(a)
        elif o in ("--rounds"):
            rounds = int(a)
        elif o in ("--save"):
            save = True
            save_path = a
        elif o in ("--load"):
            load = True
            load_path = a
        elif o in ("--train"):
            if not a in ["bpm", "beats"]:
                raise Exception('invalid training type: ' + a)
            train_type = a
        elif o in ("--evaluate"):
            eval = True
            eval_sample = make_tuple(a)
            batch_size = eval_sample[1]
        else:
            assert False, "unhandled option"
    print("loading MTT from: " + mtt_path)
    
    model = None
    
    if not eval:    
        columns = None

        if train_type == "bpm":
            if load:
                model.load_weights(load_path+"bpm")   
            model = buildBPMModel(batch_size)
        if train_type == "beats":
            model = buildBeatsModel(batch_size)
            if load:
                model.load_weights(load_path+"beats")
    
        model.summary()
        for i in range(0, rounds):
            # (annotations, clip_info, features, audio)
            base = i * data_size * 3
            print("loading training data")
            training_df = load_mtt(mtt_path, True, True, base, base + data_size, 1, verbose=verbose)
            x_train = None
            y_train = None
                
            print("loading validation data")
            validation_df = load_mtt(mtt_path, True, True, base + data_size, base + data_size * 2, 1, verbose=verbose)
            x_validate = None
            y_validate = None
                
            print("loading test data")
            test_df = load_mtt(mtt_path, True, True, base + data_size * 2, base + data_size * 3, 1, verbose=verbose)
            x_test = None
            y_test = None
            
            if train_type == "bpm":
                x_train = reshape(np.array(training_df[3].values))
                y_train = np.array(training_df[2].ix[:,2:3].values) 
                x_validate = reshape(np.array(validation_df[3].values))
                y_validate = np.array(validation_df[2].ix[:,2:3].values)
                x_test = reshape(np.array(test_df[3].values))
                y_test = np.array(test_df[2].ix[:,2:3].values)
            if train_type == "beats":
                x_train = reshape(np.array(training_df[3].values))
                y_train = np.array(training_df[2].ix[:,0:2].values)
                x_validate = reshape(np.array(validation_df[3].values))
                y_validate = np.array(validation_df[2].ix[:,0:2].values)
                x_test = reshape(np.array(test_df[3].values))
                y_test = np.array(test_df[2].ix[:,0:2].values)
                
            print("training")
            model.fit(x_train, y_train, batch_size=100, epochs=5,
                      validation_data=(x_validate, y_validate))
                
            print("evaluating")
            score = model.evaluate(x_test, y_test, batch_size=100)
            print("")
            print("metric: " + str(model.metrics_names))
            print("score : " + str(score))
            
            if save:
                if train_type == "bpm":
                    model.save_weights(save_path+"bpm")
                if train_type == "beats":
                    model.save_weights(save_path+"beats")
            
    if eval:
        bpm_model = buildBPMModel(batch_size)
        beats_model = buildBeatsModel(batch_size)
        bpm_model.load_weights(load_path+"bpm")   
        #beats_model.load_weights(load_path+"beats")
        data = load_mtt(mtt_path, True, True, eval_sample[0], eval_sample[0] + eval_sample[1], 1, verbose = verbose)
        print("evaluating")
        bpm_predictions = bpm_model.predict(reshape(np.array(data[3].values)), batch_size=eval_sample[1], verbose=verbose)
        beats_predictions = beats_model.predict(reshape(np.array(data[3].values)), batch_size=eval_sample[1], verbose=verbose)

        actual_bpm = np.array(data[2].ix[:,2:3].values)
        actual_beats = np.array(data[2].ix[:,0:2].values)
        
        print(bpm_predictions)
        print(actual_bpm)
        
        print(beats_predictions)
        print(actual_beats)

        f, (ax1, ax2) = plt.subplots(2, sharex=True, sharey=True)
        ax1.set_title('predictions')
        ax1.plot(bpm_predictions)
        ax1.plot(beats_predictions)
        ax2.set_title('actual')
        ax2.plot(actual_bpm)
        ax2.plot(actual_beats)
        plt.show()
    
def reshape(ds):
    return np.array(np.reshape(ds, (ds.shape[0],1,ds.shape[1])))
 
if __name__ == "__main__":
    main(sys.argv)