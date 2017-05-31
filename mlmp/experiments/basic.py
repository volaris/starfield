from keras.models import Sequential
from keras.layers import LSTM, Dense, Conv1D
from keras import backend
import numpy as np
from load_mtt import load_mtt
import sys, getopt
from ast import literal_eval as make_tuple

def buildModel(batch_size):
    #backend.set_image_dim_ordering('tf')
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
    model.add(LSTM(32, return_sequences=True, stateful=True))
    model.add(LSTM(32, return_sequences=True, stateful=True))
    model.add(LSTM(32, stateful=True))
    model.add(Dense(3, activation='softmax'))

    model.compile(loss='squared_hinge',
                  optimizer='rmsprop',
                  metrics=['accuracy'])
    
    return model

def main(args):
    try:
        opts, args = getopt.getopt(sys.argv[1:], "p:v", [ "path=", "data_size=", "rounds=", "save=", "load=", "evaluate=" ])
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
        elif o in ("--evaluate"):
            eval = True
            eval_sample = make_tuple(a)
            batch_size = eval_sample[1]
        else:
            assert False, "unhandled option"
    print("loading MTT from: " + mtt_path)
    
    model = buildModel(batch_size)
    
    model.summary()
    
    if load:
        model.load_weights(load_path)
    
    if not eval:
        for i in range(0, rounds):
            # (annotations, clip_info, features, audio)
            base = i * data_size * 3
            print("loading training data")
            training_df = load_mtt(mtt_path, True, True, base, base + data_size, 1, verbose=verbose)
            x_train = reshape(np.array(training_df[3].values))
            y_train = np.array(training_df[2].values)
            print("loading validation data")
            validation_df = load_mtt(mtt_path, True, True, base + data_size, base + data_size * 2, 1, verbose=verbose)
            x_validate = reshape(np.array(validation_df[3].values))
            y_validate = np.array(validation_df[2].values)
            print("loading test data")
            test_df = load_mtt(mtt_path, True, True, base + data_size * 2, base + data_size * 3, 1, verbose=verbose)
            x_test = reshape(np.array(test_df[3].values))
            y_test = np.array(test_df[2].values)
        
            print("training")
            model.fit(x_train, y_train, batch_size=100, epochs=5,
                      validation_data=(x_validate, y_validate))
                
            print("evaluating")
            score = model.evaluate(x_test, y_test, batch_size=100)
            print("")
            print("metric: " + str(model.metrics_names))
            print("score : " + str(score))
            
    if eval:
        data = load_mtt(mtt_path, True, True, eval_sample[0], eval_sample[0] + eval_sample[1], 1, verbose = verbose)
        print("evaluating")
        predictions = model.predict(reshape(np.array(data[3].values)), batch_size=eval_sample[1], verbose=verbose)
        print(predictions)
        print(np.array(data[2].values))
        
    if save:
        model.save_weights(save_path)
    
def reshape(ds):
    return np.array(np.reshape(ds, (ds.shape[0],1,ds.shape[1])))
 
if __name__ == "__main__":
    main(sys.argv)